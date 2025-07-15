using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public sealed class InvalidConfigurationException(
    string sectionName,
    string? value,
    string? extraInformation = null,
    Exception? innerException = null)
    : GuardRailDeviceException(
        $"The section '{sectionName}' with the value '{value}' was invalid.{extraInformation}",
        innerException);