using System;
using System.Collections.Generic;

namespace GuardRail.LocalClient.Data.Local
{
    public class Account
    {
        public int Id { get; set; }
        
        public Guid Guid { get; set; }
        
        public string Name { get; set; }

        public List<User> Users { get; } = new List<User>();
    }
}