using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.ApiModels.Requests;
using GuardRail.Mobile.Client.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;
using Plugin.Fingerprint.Abstractions;
using Plugin.NFC;

namespace GuardRail.Mobile.Client.Implementations;

/// <inheritdoc />
public sealed class NfcHandler(
    IGuardRailHttpClient guardRailHttpClient,
    IGeolocation geolocation,
    IFingerprint fingerprint,
    ILogger<NfcHandler> logger)
    : INfcHandler
{
    public async Task<bool> HandleTagTrigger(
        Guid doorId,
        CancellationToken cancellationToken) =>
        await HandleAuth(
            cancellationToken)
        && await HandleGeoLocation(
            doorId,
            cancellationToken)
        && await TriggerUnLock(
            doorId,
            cancellationToken);

    private async ValueTask<bool> HandleAuth(
        CancellationToken cancellationToken)
    {
        try
        {
            var authType = await fingerprint.GetAuthenticationTypeAsync();
            switch (authType)
            {
                case AuthenticationType.Fingerprint or AuthenticationType.Fingerprint:
                    {
                        var authResult = await fingerprint.AuthenticateAsync(
                            new AuthenticationRequestConfiguration(
                                "Authorize",
                                "Authorize Access")
                            {
                                FallbackTitle = "Use PIN",
                                AllowAlternativeAuthentication = true,
                                ConfirmationRequired = true
                            },
                            cancellationToken);
                        if (authResult.Status == FingerprintAuthenticationResultStatus.Succeeded)
                        {
                            return true;
                        }

                        logger.LogError("Bad auth: " + authResult.Status.ToString("G"));
                        return false;
                    }
                case AuthenticationType.None:
                    logger.LogError("No auth!!");
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(authType));
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            return false;
        }
    }

    private async ValueTask<bool> HandleGeoLocation(
        Guid doorId,
        CancellationToken cancellationToken)
    {
        var location = await geolocation.GetLocationAsync(
            new GeolocationRequest(
                GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(10)),
            cancellationToken);
        if (location == null)
        {
            return false;
        }

        return await guardRailHttpClient.PostSecureData<bool, GeoLocationRequest>(
            "/api/doors/IsInGeoFence",
            new GeoLocationRequest(
                doorId,
                location.Latitude,
                location.Longitude),
            cancellationToken);
    }

    private async ValueTask<bool> TriggerUnLock(
        Guid doorId,
        CancellationToken cancellationToken)
    {
        var newToken = await guardRailHttpClient.GetSecureData<Guid>(
            "/api/users/CreateNewToken",
            cancellationToken);
        return await guardRailHttpClient.PostSecureData<bool, object>(
            "/api/device_actions/UnLockRequest",
            new
            {
                AccessPointGuid = doorId,
                UnlockRequestType = "MobileNfc",
                Data = Encoding.UTF8.GetBytes(newToken.ToString())
            },
            cancellationToken);
    }

    public static void Current_OnMessageReceived(ITagInfo taginfo)
    {
        var message = taginfo.Records[0].Message;
        Console.WriteLine(message);
    }

    public static void Current_OnMessagePublished(ITagInfo taginfo)
    {
        var message = taginfo.Records[0].Message;
        Console.WriteLine(message);
    }

    public static void Current_OnTagDiscovered(ITagInfo taginfo, bool format)
    {
        var message = taginfo.Records[0].Message;
        Console.WriteLine(message);
    }

    public static void Current_OnTagListeningStatusChanged(bool islistening)
    {

    }

    public static void Current_OnNfcStatusChanged(bool isenabled)
    {

    }

    public static void Current_OniOSReadingSessionCancelled(object? sender, EventArgs e)
    {

    }
}