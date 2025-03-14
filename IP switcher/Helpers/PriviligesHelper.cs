using System.Security.Principal;

namespace TTech.IP_Switcher.Helpers;

internal static class PrivilegesHelper
{
    public static bool IsAdministrator()
    {
        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}