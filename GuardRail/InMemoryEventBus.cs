using System.Collections.ObjectModel;
using GuardRail.Core;

namespace GuardRail
{
    public sealed class InMemoryEventBus : IEventBus
    {
        public ObservableCollection<AccessAuthorizationEvent> AccessAuthorizationEvents { get; }

        public InMemoryEventBus()
        {
            AccessAuthorizationEvents = new ObservableCollection<AccessAuthorizationEvent>();
        }
    }
}