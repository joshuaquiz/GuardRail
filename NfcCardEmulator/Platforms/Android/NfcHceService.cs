using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using NfcCardEmulator.Services;
using System.Text;
using Android.App;
using Android.Nfc.CardEmulators;
using Android.OS;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace NfcCardEmulator.Platforms.Android;

[Service(
    Exported = true,
    Permission = "android.permission.BIND_NFC_SERVICE")]
[IntentFilter(["android.nfc.cardemulation.action.HOST_APDU_SERVICE"])]
[MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/apduservice")]
public class NfcHceService : HostApduService
{
    private const string SelectApduHeader = "00A40400";
    private const string SuccessResponse = "9000";
    private const string ErrorAuthFailed = "6300";
    private const string ErrorConditionsNotSatisfied = "6985";
    private const string ErrorSecurityNotSatisfied = "6A82";
    private const string ErrorIncorrectParameters = "6A80";

    private ILogger<NfcHceService>? _logger;
    private IBiometricService? _biometricService;
    private HttpClient? _apiService;
    private ISettingsService? _settingsService;
    private INfcService? _nfcService;

    public override void OnCreate()
    {
        base.OnCreate();
        InitializeServices();
        _logger?.LogInformation("NFC HCE Service created");
    }

    private void InitializeServices()
    {
        try
        {
            var serviceProvider = MauiApplication.Current?.Services;
            if (serviceProvider != null)
            {
                _logger = serviceProvider.GetService<ILogger<NfcHceService>>();
                _biometricService = serviceProvider.GetService<IBiometricService>();
                _apiService = serviceProvider.GetService<HttpClient>();
                _settingsService = serviceProvider.GetService<ISettingsService>();
                _nfcService = serviceProvider.GetService<INfcService>();
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing services: {ex.Message}");
        }
    }

    private bool IsSelectApdu(string apduHex)
    {
        if (apduHex.Length < 10) return false;

        var header = apduHex.Substring(0, 8);
        if (header != SelectApduHeader) return false;

        // Extract AID from APDU and compare with configured AID
        var settings = _settingsService?.GetSettings();
        var expectedAid = settings?.NfcApplicationId ?? "A000000123456789";

        // The AID starts at byte 5 (index 10 in hex string)
        if (apduHex.Length >= 10 + expectedAid.Length)
        {
            var receivedAid = apduHex.Substring(10, expectedAid.Length);
            return receivedAid.Equals(expectedAid, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private byte[] ProcessSelectApdu(string apduHex)
    {
        try
        {
            // For SELECT APDU, we need to handle the full flow synchronously
            // This is a limitation of the HCE service - we can't make it truly async
            var result = HandleNfcInteractionAsync().GetAwaiter().GetResult();
            return result;
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "Error processing SELECT APDU");
            return HexStringToByteArray(ErrorIncorrectParameters);
        }
    }

    private async Task<byte[]> HandleNfcInteractionAsync()
    {
        try
        {
            //_nfcService?.UpdateStatus(Models.NfcStatus.AwaitingFingerprint, "Waiting for biometric authentication");

            // Step 1: Biometric Authentication
            var authResult = await _biometricService?.AuthenticateAsync("Authenticate to proceed with NFC transaction")!;

            if (!authResult)
            {
                _logger?.LogWarning("Biometric authentication failed");
                //_nfcService?.UpdateStatus(Models.NfcStatus.Error, "Authentication failed");
                return HexStringToByteArray(ErrorAuthFailed);
            }

            _logger?.LogInformation("Biometric authentication successful");
            //_nfcService?.UpdateStatus(Models.NfcStatus.FetchingData, "Fetching data from API");

            // Step 2: Fetch data from API
            var settings = _settingsService?.GetSettings();
            if (settings == null)
            {
                _logger?.LogError("Settings not available");
                return HexStringToByteArray(ErrorIncorrectParameters);
            }

            var data = await _apiService?.GetFromJsonAsync<string>(
                settings.ApiEndpointUrl)!;

            if (string.IsNullOrEmpty(data))
            {
                _logger?.LogError("No data received from API");
                //_nfcService?.UpdateStatus(Models.NfcStatus.Error, "No data received from API");
                return HexStringToByteArray(ErrorIncorrectParameters);
            }

            _logger?.LogInformation($"Data fetched successfully: {data}");
            //_nfcService?.UpdateStatus(Models.NfcStatus.Transmitting, "Transmitting data via NFC");

            // Step 3: Format and return data
            var responseData = Encoding.UTF8.GetBytes(data);
            var response = new byte[responseData.Length + 2];
            Array.Copy(responseData, 0, response, 0, responseData.Length);

            // Append success status word (9000)
            var successBytes = HexStringToByteArray(SuccessResponse);
            Array.Copy(successBytes, 0, response, responseData.Length, 2);

            //_nfcService?.UpdateStatus(Models.NfcStatus.Success, $"Data transmitted successfully: {data}");
            _logger?.LogInformation("NFC transaction completed successfully");

            return response;
        }
        catch (TimeoutException)
        {
            _logger?.LogError("API request timed out");
            //_nfcService?.UpdateStatus(Models.NfcStatus.Error, "API request timed out");
            return HexStringToByteArray(ErrorIncorrectParameters);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "Error in NFC interaction flow");
            //_nfcService?.UpdateStatus(Models.NfcStatus.Error, $"Error: {ex.Message}");
            return HexStringToByteArray(ErrorIncorrectParameters);
        }
    }

    public override void OnDeactivated(DeactivationReason reason)
    {
        _logger?.LogInformation($"HCE Service deactivated. Reason: {reason}");
        //_nfcService?..UpdateStatus(Models.NfcStatus.Ready, "NFC Ready: Tap reader");
        //base.OnDeactivated(reason);
    }

    public override byte[]? ProcessCommandApdu(byte[]? commandApdu, Bundle? extras)
    {
        try
        {
            _logger?.LogInformation($"Received APDU: {BitConverter.ToString(commandApdu)}");

            var apduHex = BitConverter.ToString(commandApdu).Replace("-", "");

            // Check if this is a SELECT APDU for our AID
            if (IsSelectApdu(apduHex))
            {
                _logger?.LogInformation("SELECT APDU received, starting authentication flow");
                return ProcessSelectApdu(apduHex);
            }

            // Handle other APDUs if needed
            _logger?.LogWarning($"Unhandled APDU: {apduHex}");
            return HexStringToByteArray(ErrorIncorrectParameters);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "Error processing APDU");
            return HexStringToByteArray(ErrorIncorrectParameters);
        }
    }

    private static byte[] HexStringToByteArray(string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
}
