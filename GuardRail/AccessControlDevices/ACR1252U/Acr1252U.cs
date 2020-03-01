using System;
using System.Threading.Tasks;
using GuardRail.Definitions;
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;

namespace GuardRail.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// ACR1252U access control device.
    /// </summary>
    public sealed class Acr1252U : IAccessControlDevice
    {
        private readonly string _id;
        private readonly ISCardContext _sCardContext;
        private readonly ISCardReader _sCardReader;

        private bool _disposed;

        /// <summary>
        /// Fires when a new access request is triggered.
        /// </summary>
        public event AccessRequestedEventHandlerAsync AccessRequestedEvent;

        private Acr1252U(
            string id,
            ISCardContext sCardContext,
            Func<ISCardContext, ISCardReader> createSCardReader)
        {
            _id = id;
            _sCardContext = sCardContext;
            _sCardContext.Establish(SCardScope.System);
            _sCardReader = createSCardReader(_sCardContext);
        }

        /// <summary>
        /// Destructor for Acr1252U.
        /// </summary>
        ~Acr1252U()
        {
            Dispose(true);
        }

        /// <summary>
        /// Creates a new Acr1252U.
        /// </summary>
        /// <param name="id">The ID of the reader.</param>
        /// <returns></returns>
        public static IAccessControlDevice Create(string id) =>
            new Acr1252U(
                id,
                new SCardContext(),
                c => new SCardReader(c));

        public Task Init()
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

            foreach (var x in receiveBuffer)
            {
                Console.Write($"{x:X2} ");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns the ID if this reader.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetDeviceId() =>
            Task.FromResult(_id);

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
                _sCardContext.Dispose();
            }

            _disposed = true;
        }
    }
}