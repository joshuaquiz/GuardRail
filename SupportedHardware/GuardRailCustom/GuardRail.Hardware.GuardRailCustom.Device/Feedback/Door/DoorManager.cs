using GuardRail.Hardware.GuardRailCustom.Device.Implementations.Door;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Door;

public sealed class DoorManager(
    IDoorConfiguration<int> doorConfiguration,
    ILockableDoorHardwareManager<int> lockableDoorHardwareManager,
    ILogger<DoorManager> logger)
    : CoreDoorManager<DoorManager, int>(
        doorConfiguration,
        lockableDoorHardwareManager,
        logger);