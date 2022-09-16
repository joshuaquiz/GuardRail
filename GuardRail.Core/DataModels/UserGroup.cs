using System;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// A group of users in the system.
    /// </summary>
    public class UserGroup : IAddableItem
    {
        /// <inheritdoc />
        public Guid Guid { get; set; }
        
        /// <summary>
        /// The name of the group.
        /// </summary>
        public string Name { get; set; }
    }
}