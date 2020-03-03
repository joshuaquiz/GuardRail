using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;

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
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _watcherTask;
        private bool _disposed;

        private Acr1252UAccessControlDevice(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext,
            Func<ISCardContext, ISCardReader> createSCardReader)
        {
            _id = id;
            _eventBus = eventBus;
            _sCardContext = sCardContext;
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
        /// <returns></returns>
        public static IAccessControlDevice Create(
            string id,
            IEventBus eventBus,
            ISCardContext sCardContext)
        {
            if (sCardContext == null)
            {
                throw new ArgumentNullException(nameof(sCardContext));
            }

            return new Acr1252UAccessControlDevice(
                id,
                eventBus,
                sCardContext,
                c => new SCardReader(c));
        }

        /// <summary>
        /// Starts the watcher process.
        /// </summary>
        /// <returns></returns>
        public Task Init()
        {
            _watcherTask = new Task(
                () =>
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
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {

                        var receiveBuffer = new byte[256];
                        error = _sCardReader.Transmit(
                            sCardPci,
                            new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x0A, 0xA0, 0x00, 0x00, 0x00, 0x62, 0x03, 0x01, 0x0C, 0x06, 0x01 },
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
                },
                _cancellationTokenSource.Token);

            return Task.CompletedTask;
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