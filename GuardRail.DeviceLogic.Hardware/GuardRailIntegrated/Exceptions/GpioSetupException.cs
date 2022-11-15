using System;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Exceptions;

public sealed class GpioSetupException : Exception
{
    public GpioSetupException(string message)
        : base(message)
    {
    }
}