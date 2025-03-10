using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Mobile.Client.Interfaces;

public interface INfcHandler
{
    public Task<bool> HandleTagTrigger(
        Guid doorId,
        CancellationToken cancellationToken);
}