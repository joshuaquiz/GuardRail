using System;
using GuardRail.ApiModels.Responses;
using Microsoft.Maui.Devices;

namespace GuardRail.Mobile.Client.Implementations;

public static class FakeData
{
    public static MobileLoginResponse GetAppUsersProfile() =>
        new(
            Guid.NewGuid(),
            true,
            "Galloway",
            new Uri(
                DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7026"
                    : "https://localhost:7026",
                UriKind.Absolute));
}