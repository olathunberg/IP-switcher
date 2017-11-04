using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TTech.IP_Switcher.Features.IpSwitcher.Resources;

namespace TTech.IP_Switcher.Helpers
{
    public static class PublicIpHelper
    {
        internal static async Task<string> GetExternalIp()
        {
            var uriList = new string[] { "http://myexternalip.com/raw", "http://ifconfig.me" };

            string publicIPAddress = null;
            foreach (var uri in uriList)
            {
                publicIPAddress = await RequestExtenalIp(uri);
                publicIPAddress = publicIPAddress?.Replace("\n", "");

                if (publicIPAddress != null && ValidateStringAsIpAddress(publicIPAddress))
                    break;
            }

            if (publicIPAddress != null && ValidateStringAsIpAddress(publicIPAddress))
                return publicIPAddress;
            else
                return IpSwitcherViewModelLoc.SearchFailed;
        }

        private static async Task<string> RequestExtenalIp(string uri)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;

            request.UserAgent = "curl"; // this simulate curl linux command

            string publicIPAddress;

            request.Method = "GET";

            try
            {
                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                publicIPAddress = null;
            }

            return publicIPAddress;
        }

        private static bool ValidateStringAsIpAddress(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4 && IPAddress.TryParse(value, out IPAddress ipAddr);
        }

    }
}
