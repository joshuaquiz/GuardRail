using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models;

/// <summary>
/// Data needed for an unlock request command.
/// </summary>
/// <param name="EventTime">The time the request was created.</param>
/// <param name="Expiry">How long the request is valid for.</param>
/// <param name="UnlockTriggerType">The method used to trigger this unlock.</param>
/// <param name="TriggerData">The data from the unlock method.</param>
/// <param name="DeviceId">The ID of the device that triggered this unlock.</param>
/// <param name="LocationId">The ID of the location the device is at.</param>
public sealed record UnlockRequestCommandData(
    DateTimeOffset EventTime,
    TimeSpan Expiry,
    UnlockTriggerType UnlockTriggerType,
    string TriggerData,
    string DeviceId,
    Guid LocationId);