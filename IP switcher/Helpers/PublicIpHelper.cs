using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TTech.IP_Switcher.Features.IpSwitcher.Resources;

namespace TTech.IP_Switcher.Helpers;


public static class PublicIpHelper
{
    private static HttpClient _sharedClient = new();
    private static readonly char[] separator = ['.'];

    internal static async Task<string> GetExternalIp()
    {
        var uriList = ConfigurationManager.AppSettings["publicIpUrls"]?.Split(';');

        if (uriList == null)
            return null;

        string publicIPAddress = null;
        foreach (var uri in uriList)
        {
            publicIPAddress = await RequestExternalIp(uri);
            publicIPAddress = publicIPAddress?.Replace("\n", "");

            if (publicIPAddress != null && ValidateStringAsIpAddress(publicIPAddress))
                break;
        }

        if (publicIPAddress != null && ValidateStringAsIpAddress(publicIPAddress))
            return publicIPAddress;
        else
            return IpSwitcherViewModelLoc.SearchFailed;
    }

    private static async Task<string> RequestExternalIp(string uri)
    {
        try
        {
            var response = await _sharedClient.GetAsync(uri);
            
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }
        catch (Exception)
        {
            // Intentionally left blank
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
