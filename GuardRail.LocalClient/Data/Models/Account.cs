using System;
using System.Collections.Generic;
using GuardRail.LocalClient.Data.Interfaces;

namespace GuardRail.LocalClient.Data.Models
{
    public class Account : IAddableItem
    {
        public int Id { get; set; }

        /// <inheritdoc />
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public List<User> Users { get; } = new List<User>();
    }
}