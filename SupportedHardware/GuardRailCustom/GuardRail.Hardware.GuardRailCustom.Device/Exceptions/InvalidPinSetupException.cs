using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public sealed class InvalidPinSetupException(
    string message)
    : GuardRailDeviceException(
        message);