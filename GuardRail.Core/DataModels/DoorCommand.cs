using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// A command to a door, and all the things you can do with it.
    /// </summary>
    public sealed class DoorCommand
    {
        /// <summary>
        /// The ID of the door.
        /// </summary>
        public Guid DoorId { get; set; }

        /// <summary>
        /// Whether or not the door is locked.
        /// </summary>
        public LockedStatus LockedStatus { get; set; }

        /// <summary>
        /// How long to buzz the door.
        /// </summary>
        public TimeSpan? BuzzerDuration { get; set; }

        /// <summary>
        /// How long to turn on a red light.
        /// </summary>
        public TimeSpan? RedLightDuration { get; set; }

        /// <summary>
        /// How long to turn on a green light.
        /// </summary>
        public TimeSpan? GreenLightDuration { get; set; }
    }
}