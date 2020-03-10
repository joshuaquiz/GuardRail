using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GuardRail.Api.Devices;
using GuardRail.Core;
using Serilog;

namespace GuardRail.Api.DeviceProviders
{
    /// <summary>
    /// Logs events, that's it.
    /// </summary>
    public sealed class LoggerDeviceProvider : IDeviceProvider
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public LoggerDeviceProvider(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a logging device.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public IDevice GetDeviceByByteId(IReadOnlyList<byte> bytes)
        {
            var ints = string.Join(" ", bytes.Select(x => x.ToString("D", CultureInfo.InvariantCulture)));
            var hex = string.Join(" ", bytes.Select(x => x.ToString("X", CultureInfo.InvariantCulture)));
            _logger.Debug($"A request was made for a device (ID as ints {ints}) (ID as hex {hex})");
            return new ConsoleLoggerDevice();
        }
    }
}