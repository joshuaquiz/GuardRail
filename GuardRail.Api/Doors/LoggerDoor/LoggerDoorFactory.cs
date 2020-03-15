using System.Collections.Generic;
using System.Collections.ObjectModel;
using GuardRail.Core;
using Serilog;

namespace GuardRail.Api.Doors.LoggerDoor
{
    public sealed class LoggerDoorFactory : IDoorFactory
    {
        private readonly ILogger _logger;

        public LoggerDoorFactory(
            ILogger logger)
        {
            _logger = logger;
        }

        public IReadOnlyCollection<IDoor> GetDoors() =>
            new ReadOnlyCollection<IDoor>(
                new List<IDoor>
                {
                    new LoggerDoor(_logger)
                });
    }
}