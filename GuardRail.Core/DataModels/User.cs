using System;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// A user in the system.
    /// </summary>
    public class User : IAddableItem
    {
        /// <inheritdoc />
        public Guid Guid { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The user's phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        public string Email { get; set; }
    }
}