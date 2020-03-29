using System;

namespace GuardRail.Api.Controllers.Models
{
    public sealed class DeviceModel
    {
        public Guid Id { get; set; }

        public string FriendlyName { get; set; }

        public bool IsConfigured { get; set; }

        public UserModel User { get; set; }
    }
}