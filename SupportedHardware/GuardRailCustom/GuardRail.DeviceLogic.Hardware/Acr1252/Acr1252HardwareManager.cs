using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;

namespace GuardRail.DeviceLogic.Hardware.Acr1252;

public sealed class Acr1252HardwareManager
    : INfcHardwareManager,
        ILightHardwareManager<int>,
        IBuzzerHardwareManager<int>
{
    private readonly INfcConfiguration _nfcConfiguration;
    private readonly ILightConfiguration<int> _lightConfiguration;
    private readonly IBuzzerConfiguration<int> _buzzerConfiguration;
    private readonly ISCardContext _sCardContext;
    private readonly ILogger<Acr1252HardwareManager> _logger;

    private readonly List<Acr1252HardwareItem> _devices;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Acr1252HardwareManager(
        INfcConfiguration nfcConfiguration,
        ILightConfiguration<int> lightConfiguration,
        IBuzzerConfiguration<int> buzzerConfiguration,
        ISCardContext sCardContext,
        ILogger<Acr1252HardwareManager> logger)
    {
        _nfcConfiguration = nfcConfiguration;
        _lightConfiguration = lightConfiguration;
        _buzzerConfiguration = buzzerConfiguration;
        _sCardContext = sCardContext;
        _logger = logger;
        _devices = new List<Acr1252HardwareItem>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public ValueTask InitAsync()
    {
        _sCardContext.Establish(SCardScope.System);
        var readers = _sCardContext.GetReaders();
        _devices.AddRange(
            readers
                .Where(x =>
                    x.Contains("PICC", StringComparison.InvariantCultureIgnoreCase)
                    || x.Contains("SAM", StringComparison.InvariantCultureIgnoreCase))
                .Select(x =>
                    Acr1252HardwareItem.Create(
                        x,
                        _sCardContext,
                        _logger,
                        _cancellationTokenSource.Token,
                        Submit)));
        _sCardContext.Release();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask TurnLightOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        foreach (var device in _devices)
        {
            device.TurnLightOn(address);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask TurnLightOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        foreach (var device in _devices)
        {
            device.TurnLightOff(address);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask TurnBuzzerOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        foreach (var device in _devices)
        {
            device.TurnBeepOn(0x52);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask TurnBuzzerOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        foreach (var device in _devices)
        {
            device.TurnBeepOff(0x52);
        }

        return ValueTask.CompletedTask;
    }

    public event Func<string, CancellationToken, ValueTask>? Submit;

    public async ValueTask DisposeAddressAsync(int address)
    {
        _cancellationTokenSource.Cancel();
        foreach (var listener in _devices)
        {
            await listener.DisposeAsync();
        }
    }

    public ValueTask DisposeAddressAsync(
        string address)
    {
        _cancellationTokenSource.Cancel();
        foreach (var listener in _devices)
        {
            listener.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    private sealed class Acr1252HardwareItem : IDisposable, IAsyncDisposable
    {
        private readonly string _id;
        private readonly ISCardContext _sCardContext;
        private readonly ISCardReader _sCardReader;
        private readonly ILogger<Acr1252HardwareManager> _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly Func<string, CancellationToken, ValueTask>? _submitFunction;

        private Task? _watcherTask;
        private bool _disposed;

        private Acr1252HardwareItem(
            string id,
            ISCardContext sCardContext,
            ILogger<Acr1252HardwareManager> logger,
            CancellationToken cancellationToken,
            Func<string, CancellationToken, ValueTask>? submitFunction)
        {
            _id = id;
            _sCardContext = sCardContext;
            _logger = logger;
            _cancellationToken = cancellationToken;
            _submitFunction = submitFunction;
            _sCardContext.Establish(SCardScope.System);
            _sCardReader = new SCardReader(_sCardContext);
        }

        /// <summary>
        /// Destructor for <see cref="Acr1252HardwareItem"/>.
        /// </summary>
        ~Acr1252HardwareItem() =>
            DisposeAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Creates a new <see cref="Acr1252HardwareItem"/>.
        /// </summary>
        /// <param name="id">The ID of the reader.</param>
        /// <param name="sCardContext">The context for connecting to the card reader.</param>
        /// <param name="logger">The logger, because.. well, logs :)</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="submitFunction">The function to call when a submit is triggered.</param>
        /// <returns><see cref="Acr1252HardwareItem"/></returns>
        public static Acr1252HardwareItem Create(
            string id,
            ISCardContext sCardContext,
            ILogger<Acr1252HardwareManager> logger,
            CancellationToken cancellationToken,
            Func<string, CancellationToken, ValueTask>? submitFunction)
        {
            if (sCardContext == null)
            {
                throw new ArgumentNullException(nameof(sCardContext));
            }

            var item = new Acr1252HardwareItem(
                id,
                sCardContext,
                logger,
                cancellationToken,
                submitFunction);
            item.Init();
            return item;
        }

        /// <summary>
        /// Starts the watcher process.
        /// </summary>
        /// <returns></returns>
        private void Init()
        {
            _watcherTask = new Task(
                async () =>
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            _sCardContext.Establish(SCardScope.System);
                            var error = _sCardReader.Connect(
                                _id,
                                SCardShareMode.Shared,
                                SCardProtocol.Any);
                            ErrorCheck(error);
                            TurnBeepOff(0x52);
                            var id = GetCardId();
                            if (_submitFunction is not null)
                            {
                                await _submitFunction(
                                    id.ToJson(),
                                    new CancellationTokenSource(
                                        TimeSpan.FromMilliseconds(500)).Token);
                            }

                            await WaitForDisconnect();
                        }
                        catch (PCSCException e)
                        {
                            if (e.SCardError != SCardError.RemovedCard)
                            {
                                _logger.LogGuardRailError(e);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogGuardRailError(e);
                        }

                        await Task.Delay(
                            TimeSpan.FromMilliseconds(500),
                            _cancellationToken);
                    }
                },
                _cancellationToken);
            _watcherTask.Start();
        }

        private async Task WaitForDisconnect()
        {
            var status = SCardError.Success;
            while (status == SCardError.Success)
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(500),
                    _cancellationToken);
                status = _sCardReader.Connect(
                    _id,
                    SCardShareMode.Shared,
                    SCardProtocol.Any);
            }
        }

        /// <summary>
        /// Gets the ID of the device.
        /// </summary>
        /// <returns>The ID of the device</returns>
        private IReadOnlyList<byte> GetCardId()
        {
            var result = GetData(
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

        public bool TurnLightOn(int index)
        {
            var bytearray = new byte[8];
            var bitArray = new BitArray(bytearray);
            bitArray.Set(0, false);
            bitArray.Set(1, false);
            bitArray.Set(2, false);
            bitArray.Set(3, false);
            bitArray.Set(index, true);
            bitArray.Set(index + 2, true);
            bitArray.CopyTo(bytearray, 0);
            TransmitCommand(
                0xE0,
                0x00,
                0x00,
                0x29,
                0x01,
                bytearray);
            return true;
        }

        public bool TurnLightOff(int index)
        {
            var bytearray = new byte[8];
            var bitArray = new BitArray(bytearray);
            bitArray.Set(0, false);
            bitArray.Set(1, false);
            bitArray.Set(2, false);
            bitArray.Set(3, false);
            bitArray.Set(index, false);
            bitArray.Set(index + 2, false);
            TransmitCommand(
                0xE0,
                0x00,
                0x00,
                0x29,
                0x01,
                bytearray);
            return true;
        }

        public bool TurnBeepOn(byte address)
        {
            const byte buzzerOn = 0xFF;
            return TransmitCommand(
                0xFF,
                0x00,
                0x52,
                buzzerOn,
                0x00);
        }

        public bool TurnBeepOff(byte address)
        {
            const byte buzzerOff = 0x00;
            return TransmitCommand(
                0xFF,
                0x00,
                0x52,
                buzzerOff,
                0x00);
        }

        private bool TransmitCommand(
            byte @class,
            byte ins,
            byte p1,
            byte p2,
            byte le,
            params byte[] data)
        {
            var result = GetData(@class, ins, p1, p2, le, data);
            const byte successByte1 = 0x90;
            const byte successByte2 = 0x00;
            return result[0] == successByte1
                   && result[1] == successByte2;
        }

        private byte[] GetData(
            byte @class,
            byte ins,
            byte p1,
            byte p2,
            byte le,
            params byte[]? data)
        {
            var apdu = new List<byte> { @class, ins, p1, p2, le };
            if (data != null)
            {
                apdu.AddRange(data);
            }

            var result = new byte[256];
            var answer = _sCardReader.Transmit(apdu.ToArray(), ref result);
            ErrorCheck(answer);
            return result;
        }

        /// <summary>
        /// If the result is an error then throw it.
        /// </summary>
        /// <param name="status">The status of the command.</param>
        private static void ErrorCheck(SCardError status)
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
        /// Implementing <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        private ValueTask DisposeAsync(bool disposing)
        {
            if (_disposed)
            {
                return ValueTask.CompletedTask;
            }

            if (disposing)
            {
                _sCardReader.Disconnect(SCardReaderDisposition.Leave);
                _sCardContext.Release();
                _sCardReader.Dispose();
                _watcherTask?.Dispose();
                _sCardContext.Dispose();
            }

            _disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}