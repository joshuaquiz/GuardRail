using System;
using GuardRail.Core.DataModels;

namespace GuardRail.Api.Devices
{
    /// <inheritdoc />
    public sealed class LoggerDevice : Device
    {
        private Guid _id = Guid.NewGuid();
        
        public new string DeviceId =>
            _id.ToString();
        
        public new string FriendlyName =>
            "default testing device";
        
        public new byte[] ByteId =>
            new byte[]
            {
                0,
                1,
                2,
                3
            };
    }
}