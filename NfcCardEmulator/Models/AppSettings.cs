using System;

namespace NfcCardEmulator.Models;

public class AppSettings
{
    public string ApiEndpointUrl { get; set; } = "https://yourapi.example.com/api/getNfcData";
    public string NfcApplicationId { get; set; } = "A000000123456789";
    public bool HceServiceEnabled { get; set; } = true;
    public int HttpTimeoutSeconds { get; set; } = 10;
}

public class ApiResponse
{
    public string? Payload { get; set; }
    public string? Error { get; set; }
    public bool Success { get; set; }
}

public enum NfcStatus
{
    Ready,
    AwaitingFingerprint,
    FetchingData,
    Transmitting,
    Success,
    Error,
    Disabled
}

public class NfcStatusEventArgs : EventArgs
{
    public NfcStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}
