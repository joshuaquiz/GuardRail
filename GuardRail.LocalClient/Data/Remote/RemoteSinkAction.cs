using System;

namespace GuardRail.LocalClient.Data.Remote
{
    internal sealed class RemoteSinkAction
    {
        internal RemoteSinkActionType RemoteSinkActionType { get; set; }
        
        internal Type Type { get; set; }

        internal object Item { get; set; }
        
    }
}