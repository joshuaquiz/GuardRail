using System.Linq;
using Foundation;
using GuardRail.Mobile.Client.Interfaces;
using SystemConfiguration;

namespace GuardRail.Mobile.Client;

public sealed class AppleWiFi()
    : IWiFi
{
    public string? GetSsid()
    {
        var ssid = "";
        try
        {
            StatusCode status = CaptiveNetwork.TryGetSupportedInterfaces(out string[] supportedInterfaces);
            if (status != StatusCode.OK)
            {

            }
            else
            {
                foreach (var item in supportedInterfaces)
                {
                    status = CaptiveNetwork.TryCopyCurrentNetworkInfo(item, out var info);
                    if (status != StatusCode.OK)
                    {
                        continue;
                    }
                    ssid = info[CaptiveNetwork.NetworkInfoKeySSID].ToString();
                }
            }
        }
        catch
        {

        }
        return ssid;
    }

    public bool TryConnectToNetwork(
        string ssid,
        string password)
    {
        return false;
    }
}