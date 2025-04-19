using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback;

public class CoreScreenManager<T> : IScreenManager where T : CoreScreenManager<T>
{
    protected readonly ILogger<T> Logger;

    protected CoreScreenManager(
        ILogger<T> logger)
    {
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async ValueTask DisplayMessageAsync(
        string message,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
    }

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
    }
}