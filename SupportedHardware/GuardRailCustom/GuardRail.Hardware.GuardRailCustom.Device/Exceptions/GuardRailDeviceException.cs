using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public abstract class GuardRailDeviceException
    : Exception
{
    protected GuardRailDeviceException(
        string message)
        : base(
            message)
    {
    }

    protected GuardRailDeviceException(
        string message,
        Exception? innerException)
        : base(
            message,
            innerException)
    {
    }
}