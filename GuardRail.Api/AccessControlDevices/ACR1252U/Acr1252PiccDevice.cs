using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;
using Serilog;

namespace GuardRail.Api.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// ACR1252U PICC access control device.
    /// </summary>
    public sealed class Acr1252PiccDevice : IAccessControlDevice
    {
        private readonly string _id;
        private readonly IEventBus _eventBus;
        private readonly ISCardContext _sCardContext;
        private readonly ISCardReader _sCardReader;
        private readonly ILogger _logger;
        private readonly IDeviceProvider _deviceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _watcherTask;
        private bool _disposed;

        private Acr1252PiccDevice(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger,
            IDeviceProvider deviceProvider,
            Func<ISCardContext, ISCardReader> createSCardReader)
        {
            _id = id;
            _eventBus = eventBus;
            _sCardContext = sCardContext;
            _logger = logger;
            _deviceProvider = deviceProvider;
            _sCardContext.Establish(SCardScope.System);
            _sCardReader = createSCardReader(_sCardContext);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Destructor for Acr1252U.
        /// </summary>
        ~Acr1252PiccDevice()
        {
            Dispose(true);
        }

        /// <summary>
        /// Creates a new Acr1252U.
        /// </summary>
        /// <param name="id">The ID of the reader.</param>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="sCardContext">The context for connecting to the card reader.</param>
        /// <param name="logger">The logger, because.. well, logs :)</param>
        /// <param name="deviceProvider">The provider to look up devices by their ID.</param>
        /// <returns></returns>
        public static IAccessControlDevice Create(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger,
            IDeviceProvider deviceProvider)
        {
            if (sCardContext == null)
            {
                throw new ArgumentNullException(nameof(sCardContext));
            }

            return new Acr1252PiccDevice(
                id,
                eventBus,
                sCardContext,
                logger,
                deviceProvider,
                c => new SCardReader(c));
        }

        /// <summary>
        /// Starts the watcher process.
        /// </summary>
        /// <returns></returns>
        public Task Init()
        {
            _watcherTask = new Task(
                async () =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            _sCardContext.Establish(SCardScope.System);
                            var error = _sCardReader.Connect(
                                _id,
                                SCardShareMode.Shared,
                                SCardProtocol.Any);
                            ErrorCheck(error);
                            TurnBeepOff(_sCardReader);
                            var id = GetCardId(_sCardReader);
                            var device = _deviceProvider.GetDeviceByByteId(id);
                            _eventBus.AccessAuthorizationEvents.Add(
                                AccessAuthorizationEvent.Create(
                                    device,
                                    this));
                            await WaitForDisconnect();
                        }
                        catch (PCSCException e)
                        {
                            if (e.SCardError != SCardError.RemovedCard)
                            {
                                _logger.Error(e, e.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, e.Message);
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                },
                _cancellationTokenSource.Token);
            _watcherTask.Start();
            return Task.CompletedTask;
        }

        private async Task WaitForDisconnect()
        {
            var status = SCardError.Success;
            while (status == SCardError.Success)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                status = _sCardReader.Connect(
                    _id,
                    SCardShareMode.Shared,
                    SCardProtocol.Any);
            }
        }

        /// <summary>
        /// Gets the ID of the device.
        /// </summary>
        /// <param name="cardReader"></param>
        /// <returns>The ID of the device</returns>
        public static IReadOnlyList<byte> GetCardId(ISCardReader cardReader)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            var result = GetData(
                cardReader,
                0xFF,
                0xCA,
                0x01,
                0x00,
                0x00);
            var data = result.ToList();
            // Remote the two bytes indicating success, leaving us with the bytes for the ID.
            data.Remove(data.Last());
            data.Remove(data.Last());
            return new ReadOnlyCollection<byte>(data);
        }

        public static bool TurnRedLightOn(ISCardReader cardReader)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            var bytearray = new byte[8];
            var bitArray = new BitArray(bytearray);
            bitArray.Set(0, true);
            bitArray.Set(1, false);
            bitArray.CopyTo(bytearray, 0);
            TransmitCommand(
                cardReader,
                0xE0,
                0x00,
                0x00,
                0x29,
                0x01,
                bytearray);
            return true;
        }

        public static bool TurnBeepOn(ISCardReader cardReader)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            const byte buzzerOn = 0xFF;
            return TransmitCommand(
                cardReader,
                0xFF,
                0x00,
                0x52,
                buzzerOn,
                0x00);
        }

        public static bool TurnBeepOff(ISCardReader cardReader)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            const byte buzzerOff = 0x00;
            return TransmitCommand(
                cardReader,
                0xFF,
                0x00,
                0x52,
                buzzerOff,
                0x00);
        }

        private static bool TransmitCommand(
            ISCardReader cardReader,
            byte @class,
            byte ins,
            byte p1,
            byte p2,
            byte le,
            params byte[] data)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            var result = GetData(cardReader, @class, ins, p1, p2, le, data);
            const byte successByte1 = 0x90;
            const byte successByte2 = 0x00;
            return result[0] == successByte1
                   && result[1] == successByte2;
        }

        private static byte[] GetData(
            ISCardReader cardReader,
            byte @class,
            byte ins,
            byte p1,
            byte p2,
            byte le,
            params byte[] data)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            var apdu = new List<byte> {@class, ins, p1, p2, le};
            if (data == null)
            {
                apdu.AddRange(data);
            }

            var result = new byte[256];
            var answer = cardReader.Transmit(apdu.ToArray(), ref result);
            ErrorCheck(answer);
            return result;
        }

        /// <summary>
        /// If the result is an error then throw it.
        /// </summary>
        /// <param name="status">The status of the command.</param>
        public static void ErrorCheck(SCardError status)
        {
            if (status == SCardError.Success)
            {
                return;
            }

            throw new PCSCException(
                status,
                SCardHelper.StringifyError(
                    status));
        }

        /// <summary>
        /// Returns the ID if this reader.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetDeviceId() =>
            Task.FromResult(_id);

        /// <summary>
        /// Let the user know the authentication request failed.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Task PresentNoAccessGranted(string reason)
        {
            try
            {
                TurnRedLightOn(_sCardReader);
            }
            catch (PCSCException e)
            {
                if (e.SCardError != SCardError.RemovedCard)
                {
                    _logger.Error(e, e.Message);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Implementing <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _sCardReader.Disconnect(SCardReaderDisposition.Leave);
                _sCardContext.Release();
                _sCardReader.Dispose();
                _watcherTask.Dispose();
                _cancellationTokenSource.Dispose();
                _sCardContext.Dispose();
            }

            _disposed = true;
        }
    }
}