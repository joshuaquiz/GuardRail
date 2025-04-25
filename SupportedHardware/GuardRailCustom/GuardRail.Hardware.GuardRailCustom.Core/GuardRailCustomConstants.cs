namespace GuardRail.Hardware.GuardRailCustom.Core;

public static class GuardRailCustomConstants
{
    public const string ServiceName = "GuardRail";
    public const string UdpSeparator = "~";
    public const int UdpDiscoveryPort = 50005;
    public const int UdpCommandPort = 12345;

    public static class UdpCommandNames
    {
        public const string Sync = nameof(Sync);
        public const string UnLockDoor = nameof(UnLockDoor);
    }
}