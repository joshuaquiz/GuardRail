using System;
using GuardRail.LocalClient.Data.Interfaces;

namespace GuardRail.LocalClient.Data.Models
{
    public class User : IAddableItem
    {
        public int Id { get; set; }

        /// <inheritdoc />
        public Guid Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }

        public Account Account { get; set; }
    }
}