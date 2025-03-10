using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

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