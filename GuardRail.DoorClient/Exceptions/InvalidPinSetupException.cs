using System;

namespace GuardRail.DoorClient.Exceptions
{
    public sealed class InvalidPinSetupException : Exception
    {
        public InvalidPinSetupException(string message)
            : base(message)
        {
        }
    }
}