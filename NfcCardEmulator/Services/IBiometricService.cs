using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace NfcCardEmulator.Services;

public interface IBiometricService
{
    Task<bool> IsAvailableAsync();
    Task<bool> AuthenticateAsync(string reason);
}

public class BiometricAuthResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public BiometricAuthStatus Status { get; set; }
}

public enum BiometricAuthStatus
{
    Unknown,
    Available,
    NotAvailable,
    NotEnrolled,
    Denied,
    Cancelled,
    Failed,
    Succeeded
}

#if ANDROID

public class BiometricService : IBiometricService
{
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            return await CrossFingerprint.Current.IsAvailableAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        try
        {
            if (!await IsAvailableAsync())
                return false;

            var request = new AuthenticationRequestConfiguration(
                "Biometric Authentication",
                reason)
            {
                AllowAlternativeAuthentication = true,
                CancelTitle = "Cancel",
                FallbackTitle = "Use Password"
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);
            return result.Authenticated;
        }
        catch
        {
            return false;
        }
    }
}
#else
public class BiometricService : IBiometricService
{
    public async Task<bool> IsAvailableAsync()
    {
        // iOS implementation would go here
        // For now, return false as iOS HCE is not supported for this use case
        await Task.CompletedTask;
        return false;
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        // iOS implementation would go here
        await Task.CompletedTask;
        return false;
    }
}
#endif
