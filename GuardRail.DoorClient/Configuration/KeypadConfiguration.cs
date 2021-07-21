using System;
using System.Collections.Generic;

namespace GuardRail.DoorClient.Configuration
{
    public sealed class KeypadConfiguration
    {
        public List<int> Pins { get; set; }

        public TimeSpan KeypadTimeout { get; set; }

        public char SubmitKey { get; set; }
    }
}