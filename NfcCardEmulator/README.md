# NFC Card Emulator - .NET MAUI Application

## Overview

This .NET MAUI application emulates an NFC card using Host-based Card Emulation (HCE) on Android devices. When an external NFC reader interacts with the phone, the app:

1. Requires fingerprint/biometric authentication from the user
2. Fetches data from a specified HTTP endpoint upon successful authentication
3. Transmits the fetched data to the NFC reader

## Platform Support

### Android (Full Implementation)
- ✅ Host-based Card Emulation (HCE) using `HostApduService`
- ✅ Biometric authentication using Plugin.Fingerprint
- ✅ Configurable Application ID (AID)
- ✅ Real-time status updates
- ✅ Complete APDU handling

### iOS (Limited Support)
- ❌ **HCE Not Supported**: iOS does not support general-purpose Host-based Card Emulation
- ❌ **Limitations**: Apple restricts NFC card emulation to specific use cases (Apple Pay, etc.)
- 🔄 **Alternative Approaches**:
  - Use NFC reading capabilities with Core NFC framework
  - Implement peer-to-peer communication using NFC Data Exchange Format (NDEF)
  - Consider Apple Wallet integration for specific payment/pass scenarios
  - Use QR codes or Bluetooth as alternative communication methods

## Architecture

### Shared Code (.NET MAUI)
- **Models**: `AppSettings`, `NfcStatus`, `ApiResponse`
- **Services**: 
  - `ISettingsService`: Configuration management
  - `IBiometricService`: Biometric authentication
  - `IApiService`: HTTP API communication
  - `INfcService`: NFC status management
- **ViewModels**: MVVM pattern with data binding
- **Views**: Cross-platform UI using XAML

### Platform-Specific Code (Android)
- **NfcHceService**: `HostApduService` implementation
- **AndroidManifest.xml**: Required permissions and service declarations
- **APDU Service Configuration**: XML metadata for AID registration

## Key Features

### 1. NFC Host-based Card Emulation
- Registers custom Application ID (AID): `A000000123456789` (configurable)
- Handles SELECT APDU commands
- Processes authentication and data transmission flow
- Supports APDU size limitations and error handling

### 2. Biometric Authentication
- Fingerprint authentication using Plugin.Fingerprint
- Fallback to device PIN/password if available
- Secure authentication before data transmission
- Proper error handling for authentication failures

### 3. Dynamic Data Retrieval
- Configurable HTTP API endpoint
- JSON response parsing with configurable field extraction
- Timeout handling (configurable, default 10 seconds)
- Comprehensive error handling for network issues

### 4. User Interface
- Real-time status updates during NFC interactions
- Settings page for configuration
- Enable/disable HCE service toggle
- Test biometric authentication functionality

## Configuration

### App Settings (Configurable via UI)
```csharp
public class AppSettings
{
    public string ApiEndpointUrl { get; set; } = "https://yourapi.example.com/api/getNfcData";
    public string JsonFieldName { get; set; } = "payload";
    public string NfcApplicationId { get; set; } = "A000000123456789";
    public bool HceServiceEnabled { get; set; } = true;
    public int HttpTimeoutSeconds { get; set; } = 10;
}
```

### Required Permissions (Android)
```xml
<uses-permission android:name="android.permission.NFC" />
<uses-permission android:name="android.permission.USE_BIOMETRIC" />
<uses-permission android:name="android.permission.USE_FINGERPRINT" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

### Required Features (Android)
```xml
<uses-feature android:name="android.hardware.nfc" android:required="true" />
<uses-feature android:name="android.hardware.nfc.hce" android:required="true" />
<uses-feature android:name="android.hardware.fingerprint" android:required="false" />
```

## NFC Transaction Flow

1. **NFC Reader Detection**: External reader sends SELECT APDU with registered AID
2. **Authentication Prompt**: App immediately prompts for biometric authentication
3. **API Data Fetch**: Upon successful auth, app makes HTTP GET request to configured endpoint
4. **Data Transmission**: Extracted data is formatted as APDU response and sent to reader
5. **Status Updates**: Real-time status shown in UI throughout the process

### APDU Response Codes
- `9000`: Success
- `6300`: Authentication Failed
- `6985`: Conditions of use not satisfied
- `6A82`: Security status not satisfied
- `6A80`: Incorrect parameters in data field

## Required NuGet Packages

```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="8.0.3" />
<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="8.0.3" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="8.0.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="Plugin.Fingerprint" Version="3.0.0-beta.1" />
<PackageReference Include="Xamarin.AndroidX.Biometric" Version="1.1.0.16" />
```

## Testing

### Testing Tools
1. **Android NFC Reader Apps**:
   - NFC TagInfo by NXP
   - NFC Tools
   - TagWriter by NXP

2. **Hardware NFC Readers**:
   - ACR122U USB NFC Reader
   - PN532 NFC Module
   - Any ISO 14443 Type A/B compatible reader

### Testing Steps
1. Install app on Android device with NFC capability
2. Enable NFC in device settings
3. Configure biometric authentication (fingerprint/face)
4. Launch app and enable HCE service
5. Use NFC reader tool to send SELECT APDU with configured AID
6. Follow authentication prompts
7. Verify data transmission in reader tool

### Mock API Endpoint
For testing, create a simple API endpoint that returns:
```json
{
  "payload": "Hello from NFC Card Emulator!",
  "timestamp": "2024-01-01T12:00:00Z",
  "success": true
}
```

## Security Considerations

1. **No Hardcoded Secrets**: API keys should be configured securely
2. **Biometric Authentication**: Required before any data transmission
3. **HTTPS Only**: All API communications should use HTTPS
4. **Data Validation**: Input validation for all configuration fields
5. **Error Handling**: Secure error messages that don't leak sensitive information

## Troubleshooting

### Common Issues
1. **NFC Not Working**: Ensure device has NFC capability and it's enabled
2. **Biometric Auth Fails**: Check device biometric setup and app permissions
3. **API Timeout**: Verify network connectivity and API endpoint availability
4. **AID Not Recognized**: Ensure AID matches between app and reader configuration

### Logging
The app includes comprehensive logging for debugging:
- NFC APDU exchanges
- Authentication attempts
- API requests/responses
- Error conditions

## Future Enhancements

1. **APDU Chaining**: Support for large data transmission
2. **Multiple AIDs**: Support for multiple application IDs
3. **Secure Element**: Integration with hardware secure element
4. **iOS Alternative**: Implement NFC reading with alternative communication
5. **Encryption**: End-to-end encryption for sensitive data transmission
