using System;

namespace GuardRail.Hardware.GuardRailCustom.Device.Exceptions;

public sealed class GpioSetupException(string message) : Exception(message);