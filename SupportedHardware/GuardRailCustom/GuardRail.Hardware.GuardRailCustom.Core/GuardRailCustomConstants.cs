namespace GuardRail.Hardware.GuardRailCustom.Core;

public static class GuardRailCustomConstants
{
    public const string DiscoveryKey = "FIND";
    public const string UdpSeparator = "~";
    public const int UdpDiscoveryPort = 12345;

    public static class UdpCommandNames
    {
        public const string ConfirmConnection = nameof(ConfirmConnection);
        public const string UnLockDoor = nameof(UnLockDoor);
        public const string UnlockRequest = nameof(UnlockRequest);
    }
}