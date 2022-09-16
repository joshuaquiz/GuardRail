using System;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// An account in the system.
    /// </summary>
    public class Account : IAddableItem
    {
        /// <inheritdoc />
        public Guid Guid { get; set; }

        /// <summary>
        /// The name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location of the installation.
        /// </summary>
        public string Location { get; set; }
    }
}