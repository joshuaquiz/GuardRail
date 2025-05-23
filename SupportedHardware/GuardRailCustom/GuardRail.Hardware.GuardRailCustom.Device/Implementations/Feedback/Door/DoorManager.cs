using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Door;

public sealed class DoorManager(
    IDoorConfiguration<int> doorConfiguration,
    ILockableDoorHardwareManager<int> lockableDoorHardwareManager,
    ILogger<DoorManager> logger)
    : CoreDoorManager<DoorManager, int>(
        doorConfiguration,
        lockableDoorHardwareManager,
        logger);