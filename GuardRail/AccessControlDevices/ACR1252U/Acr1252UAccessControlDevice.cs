using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;
using Serilog;

namespace GuardRail.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// ACR1252U access control device.
    /// </summary>
    public sealed class Acr1252UAccessControlDevice : IAccessControlDevice
    {
        private readonly string _id;
        private readonly IEventBus _eventBus;
        private readonly ISCardContext _sCardContext;
        private readonly ISCardReader _sCardReader;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _watcherTask;
        private bool _disposed;

        private Acr1252UAccessControlDevice(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger,
            Func<ISCardContext, ISCardReader> createSCardReader)
        {
            _id = id;
            _eventBus = eventBus;
            _sCardContext = sCardContext;
            _logger = logger;
            _sCardContext.Establish(SCardScope.System);
            _sCardReader = createSCardReader(_sCardContext);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Destructor for Acr1252U.
        /// </summary>
        ~Acr1252UAccessControlDevice()
        {
            Dispose(true);
        }

        /// <summary>
        /// Creates a new Acr1252U.
        /// </summary>
        /// <param name="id">The ID of the reader.</param>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="sCardContext"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IAccessControlDevice Create(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger)
        {
            if (sCardContext == null)
            {
                throw new ArgumentNullException(nameof(sCardContext));
            }

            return new Acr1252UAccessControlDevice(
                id,
                eventBus,
                sCardContext,
                logger,
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
                            if (error != SCardError.Success)
                            {
                                throw new PCSCException(
                                    error,
                                    SCardHelper.StringifyError(
                                        error));
                            }

                            IntPtr sCardPci;
                            switch (_sCardReader.ActiveProtocol)
                            {
                                case SCardProtocol.T0:
                                    sCardPci = SCardPCI.T0;
                                    break;
                                case SCardProtocol.T1:
                                    sCardPci = SCardPCI.T1;
                                    break;
                                default:
                                    throw new PCSCException(
                                        SCardError.ProtocolMismatch,
                                        $"Protocol not supported: {_sCardReader.ActiveProtocol}");
                            }

                            var res = GetCardId(_sCardReader);

                            var receiveBuffer = new byte[256];
                            error = _sCardReader.Transmit(
                                sCardPci,
                                new byte[]
                                {
                                    0x00, 0xA4, 0x04, 0x00, 0x0A, 0xA0, 0x00, 0x00, 0x00, 0x62, 0x03, 0x01, 0x0C, 0x06,
                                    0x01
                                },
                                ref receiveBuffer);
                            if (error != SCardError.Success)
                            {
                                throw new PCSCException(
                                    error,
                                    SCardHelper.StringifyError(
                                        error));
                            }

                            if (receiveBuffer.Length > 0)
                            {
                                _eventBus.AccessAuthorizationEvents.Add(
                                    AccessAuthorizationEvent.Create(
                                        null,
                                        this));
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Debug(e, e.Message);
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                },
                _cancellationTokenSource.Token);
            _watcherTask.Start();
            return Task.CompletedTask;
        }

        public static bool GetCardId(ISCardReader cardReader)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            var data = GetData(
                cardReader,
                0xFF,
                0xCA,
                0x00,
                0x00,
                0x00);
            var asdf = $"{data[0]:x0} {data[1]:x0} {data[2]:x0} {data[3]:x0}";
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
            byte le)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            byte[] apdu = { @class, ins, p1, p2, le };
            var result = new byte[2];
            var answer = cardReader.Transmit(apdu, ref result);
            const byte successByte1 = 0x90;
            const byte successByte2 = 0x00;
            ErrorCheck(answer);
            return result[0] == successByte1
                   && result[1] == successByte2;
        }

        private static byte[] GetData(
            ISCardReader cardReader,
            byte @class,
            byte ins,
            byte p1,
            byte p2,
            byte le)
        {
            if (cardReader == null)
            {
                throw new ArgumentNullException(nameof(cardReader));
            }

            byte[] apdu = { @class, ins, p1, p2, le };
            var result = new byte[256];
            var answer = cardReader.Transmit(apdu, ref result);
            ErrorCheck(answer);
            return result;
        }

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
                _sCardReader.Dispose();
                _watcherTask.Dispose();
                _cancellationTokenSource.Dispose();
                _sCardContext.Dispose();
            }

            _disposed = true;
        }
    }
}