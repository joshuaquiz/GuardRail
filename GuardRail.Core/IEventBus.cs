using System.Collections.ObjectModel;

namespace GuardRail.Core;

/// <summary>
/// Event handling.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// The events.
    /// </summary>
    ObservableCollection<AccessAuthorizationEvent> AccessAuthorizationEvents { get; }
}