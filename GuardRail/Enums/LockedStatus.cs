namespace GuardRail.Enums
{
    /// <summary>
    /// The locked states for a door.
    /// </summary>
    public enum LockedStatus
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
        Locked = 2
    }
}