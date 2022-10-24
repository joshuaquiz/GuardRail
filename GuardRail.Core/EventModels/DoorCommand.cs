using System;
using System.Collections.Generic;

namespace GuardRail.Core.EventModels;

/// <summary>
/// A command to a door.
/// </summary>
/// <param name="DoorId">The ID of the door.</param>
/// <param name="DoorStateRequests">All requested state changes.</param>
/// <param name="Message">A message related to the command.</param>
public sealed record DoorCommand(
    Guid DoorId,
    IReadOnlyCollection<DoorStateRequest>? DoorStateRequests,
    string? Message);