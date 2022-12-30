using System;
using GuardRail.Core.Data.Enums;

namespace GuardRail.Core.EventModels;

/// <summary>
/// A request item for state change on a door.
/// </summary>
/// <param name="DoorStateRequestType">The type of state change.</param>
/// <param name="Duration">How long to hold the door status.</param>
public sealed record DoorStateRequest(
    DoorStateRequestType DoorStateRequestType,
    TimeSpan Duration);