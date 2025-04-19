using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public sealed class InvalidPinSetupException : Exception
{
    public InvalidPinSetupException(string message)
        : base(message)
    {
    }
}