using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public sealed class GpioSetupException : Exception
{
    public GpioSetupException(string message)
        : base(message)
    {
    }
}