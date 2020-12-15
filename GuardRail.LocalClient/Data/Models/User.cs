using System;

namespace GuardRail.LocalClient.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Account Account { get; set; }
    }
}