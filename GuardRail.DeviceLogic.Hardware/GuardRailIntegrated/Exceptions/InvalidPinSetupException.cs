using System;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Exceptions;

public sealed class InvalidPinSetupException : Exception
{
    public InvalidPinSetupException(string message)
        : base(message)
    {
    }
}