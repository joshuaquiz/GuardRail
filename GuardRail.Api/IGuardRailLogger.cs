using System;
using System.Threading.Tasks;

namespace GuardRail.Api
{
    public interface IGuardRailLogger
    {
        public Task LogAsync(string logMessage);

        public Task LogErrorAsync(Exception exception);

        public Task LogErrorAsync(string logMessage);
    }
}