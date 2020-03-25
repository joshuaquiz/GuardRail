using System;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using Serilog;
using Log = GuardRail.Api.Data.Log;

namespace GuardRail.Api
{
    public sealed class GuardRailLogger
    {
        private readonly ILogger _logger;
        private readonly GuardRailContext _guardRailContext;

        public GuardRailLogger(
            ILogger logger,
            GuardRailContext guardRailContext)
        {
            _logger = logger;
            _guardRailContext = guardRailContext;
        }

        public async Task LogAsync(string logMessage)
        {
            _logger.Debug(logMessage);
            await _guardRailContext.Logs.AddAsync(
                new Log
                {
                    DateTime = DateTimeOffset.UtcNow,
                    LogMessage = logMessage
                });
            await _guardRailContext.SaveChangesAsync();
        }

        public async Task LogErrorAsync(Exception exception)
        {
            _logger.Error(exception, exception?.Message);
            await _guardRailContext.Logs.AddAsync(
                new Log
                {
                    DateTime = DateTimeOffset.UtcNow,
                    LogMessage = exception?.Message
                });
            await _guardRailContext.SaveChangesAsync();
        }

        public async Task LogErrorAsync(string logMessage)
        {
            _logger.Error(logMessage);
            await _guardRailContext.Logs.AddAsync(
                new Log
                {
                    DateTime = DateTimeOffset.UtcNow,
                    LogMessage = logMessage
                });
            await _guardRailContext.SaveChangesAsync();
        }
    }
}