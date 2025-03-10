using System;
using System.Collections.Generic;

namespace GuardRail.Core.EventModels;

/// <summary>
/// A command to a door.
/// </summary>
/// <param name="AccessPointId">The ID of the access point.</param>
/// <param name="DoorStateRequests">All requested state changes.</param>
/// <param name="Message">A message related to the command.</param>
public sealed record DoorCommand(
    Guid AccessPointId,
    IReadOnlyCollection<DoorStateRequest>? DoorStateRequests,
    string? Message);