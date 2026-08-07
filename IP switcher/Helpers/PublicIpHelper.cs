using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TTech.IP_Switcher.Features.IpSwitcher.Resources;
using TTech.IP_Switcher.Helpers.Logging;

namespace TTech.IP_Switcher.Helpers;

public static class PublicIpHelper
{
    private static readonly HttpClient sharedClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };
    private static readonly IAppLogger appLogger = AppLogger.Instance;
    private static readonly char[] separator = ['.'];

    internal static async Task<string> GetExternalIp()
    {
        var uriList = ConfigurationManager.AppSettings["publicIpUrls"]?.Split(';');

        if (uriList == null)
            return IpSwitcherViewModelLoc.SearchFailed;

        foreach (var uri in uriList)
        {
            if (string.IsNullOrWhiteSpace(uri))
                continue;

            var publicIPAddress = await RequestExternalIp(uri.Trim());
            publicIPAddress = publicIPAddress?.Trim();

            if (ValidateStringAsIpAddress(publicIPAddress))
                return publicIPAddress;
        }

        return IpSwitcherViewModelLoc.SearchFailed;
    }

    private static async Task<string> RequestExternalIp(string uri)
    {
        try
        {
            using var response = await sharedClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }

            appLogger.LogInfo("Public IP lookup", $"Endpoint '{uri}' returned HTTP {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            appLogger.LogError("Public IP lookup", $"Endpoint '{uri}' failed.", ex);
        }

        return null;
    }

    private static bool ValidateStringAsIpAddress(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return value.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length == 4 && IPAddress.TryParse(value, out IPAddress ipAddr);
    }
}
