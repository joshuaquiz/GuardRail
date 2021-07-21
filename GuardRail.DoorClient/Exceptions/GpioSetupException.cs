using System;

namespace GuardRail.DoorClient.Exceptions
{
    public sealed class GpioSetupException : Exception
    {
        public GpioSetupException(string message)
            : base(message)
        {
        }
    }
}