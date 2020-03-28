using System;
using System.Collections.Generic;

namespace GuardRail.Api.Controllers.Models
{
    public sealed class AccessControlDeviceModel
    {
        public Guid Id { get; set; }

        public string FriendlyName { get; set; }

        public string DeviceId { get; set; }

        public bool IsConfigured { get; set; }

        public List<DoorModel> Doors { get; set; }
    }
}