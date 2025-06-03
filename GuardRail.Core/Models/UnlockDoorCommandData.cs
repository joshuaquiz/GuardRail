using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models;

/// <summary>
/// Data needed for an unlock command.
/// </summary>
/// <param name="HardwareId">The ID of the door to unlock.</param>
/// <param name="AccessPointType">The type of hardware the ID is associated with.</param>
/// <param name="UnlockDuration">The length of time for the door to remain unlocked.</param>
/// <param name="BuzzerDuration">The length of time for the buzzer to go off.</param>
/// <param name="GreenLightDuration">The length of time for the green light to remain on.</param>
public sealed record UnlockDoorCommandData(
    string HardwareId,
    AccessPointType AccessPointType,
    TimeSpan UnlockDuration,
    TimeSpan? BuzzerDuration,
    TimeSpan? GreenLightDuration);