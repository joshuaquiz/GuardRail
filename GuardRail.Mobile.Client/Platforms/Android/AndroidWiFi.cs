using System.Linq;
using Android.Net.Wifi;
using GuardRail.Mobile.Client.Interfaces;

namespace GuardRail.Mobile.Client;

public sealed class AndroidWiFi(
    WifiManager wifiManager)
    : IWiFi
{
    public string? GetSsid() =>
        wifiManager.ConnectionInfo?.SSID;

    public bool TryConnectToNetwork(
        string ssid,
        string password)
    {
        wifiManager.AddNetwork(
            new WifiConfiguration
            {
                Ssid = ssid,
                PreSharedKey = password
            });
        var network = wifiManager.ConfiguredNetworks
            .FirstOrDefault(n => n.Ssid == ssid);
        return network != null;
    }
}