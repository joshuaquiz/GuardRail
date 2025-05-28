namespace GuardRail.Mobile.Client.Interfaces;

public interface IWiFi
{
    public string? GetSsid();

    public bool TryConnectToNetwork(
        string ssid,
        string password);
}