using System;
using GuardRail.Core.Enums;

namespace GuardRail.Api.Controllers.Models;

public sealed class DoorModel
{
    public Guid Id { get; set; }

    public string FriendlyName { get; set; }

    public string DeviceId { get; set; }

    public DoorStateRequestType DoorStateRequestType{ get; set; }
}