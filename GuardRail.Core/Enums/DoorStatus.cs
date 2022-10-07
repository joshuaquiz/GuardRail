using System;

namespace GuardRail.Core.Enums
{
    /// <summary>
    /// The locked states for a door.
    /// </summary>
    [Flags]
    public enum DoorStatus : byte
    {
        /// <summary>
        /// The locked status of the door cannot be determined.
        /// </summary>
        UnKnown = 0,

        /// <summary>
        /// The door is unlocked.
        /// </summary>
        UnLocked = 1,

        /// <summary>
        /// The door is locked.
        /// </summary>
        Locked = 2,

        /// <summary>
        /// The door is open.
        /// </summary>
        Open = 4,

        /// <summary>
        /// The door is closed.
        /// </summary>
        Closed = 8
    }
}