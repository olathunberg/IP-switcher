using Microsoft.Win32;
using System;
using System.Collections.Generic;

public static class GetDotNetVersions
{
    public static IList<Version> InstalledDotNetVersions()
    {
        var versions = new List<Version>();
        var NDPKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");

        if (NDPKey != null)
        {
            string[] subkeys = NDPKey.GetSubKeyNames();
            foreach (string subkey in subkeys)
            {
                GetDotNetVersion(NDPKey.OpenSubKey(subkey), subkey, versions);
                GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Client"), subkey, versions);
                GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Full"), subkey, versions);
            }
        }
        return versions;
    }

    private static void GetDotNetVersion(RegistryKey parentKey, string subVersionName, List<Version> versions)
    {
        if (parentKey == null)
            return;

        string installed = Convert.ToString(parentKey.GetValue("Install"));
        if (installed == "1")
        {
            string version = Convert.ToString(parentKey.GetValue("Version"));
            if (string.IsNullOrEmpty(version))
            {
                if (subVersionName.StartsWith("v", StringComparison.CurrentCulture))
                    version = subVersionName.Substring(1);
                else
                    version = subVersionName;
            }

            var ver = new Version(version);

            if (!versions.Contains(ver))
                versions.Add(ver);
        }
    }
}