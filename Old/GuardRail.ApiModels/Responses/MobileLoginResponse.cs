using System;

namespace GuardRail.ApiModels.Responses;

/// <summary>
/// The result of a login from mobile.
/// </summary>
/// <param name="UserId">The ID of the logged-in user.</param>
/// <param name="UseLocalNetwork">Whether to use the local network or just the default network for secure communication.</param>
/// <param name="WiFiName">The name of the Wi-Fi network to use, if any.</param>
/// <param name="RootAddress">The base URI for all secure communication.</param>
public sealed record MobileLoginResponse(
    Guid UserId,
    bool UseLocalNetwork,
    string? WiFiName,
    Uri RootAddress);