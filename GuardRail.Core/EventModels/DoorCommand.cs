using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.EventModels
{
    /// <summary>
    /// A command to a door.
    /// </summary>
    public abstract record BaseDoorCommand
    {
        /// <summary>
        /// The ID of the door.
        /// </summary>
        public Guid DoorId { get; set; }

        /// <summary>
        /// Whether or not the door is locked.
        /// </summary>
        public DoorStatus DoorStatus { get; protected set; }

        /// <summary>
        /// How long to hold the door status.
        /// </summary>
        public TimeSpan DoorStatusDuration { get; set; }

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

        /// <summary>
        /// A message related to the command.
        /// </summary>
        public string? Message { get; set; }
    }
    /// <summary>
    /// A command to unlock a door.
    /// </summary>
    public sealed record UnLockCommand : BaseDoorCommand
    {
        public UnLockCommand(bool open = false)
        {
            DoorStatus = open
                ? DoorStatus.UnLocked | DoorStatus.Open
                : DoorStatus.UnLocked;
        }
    }
}