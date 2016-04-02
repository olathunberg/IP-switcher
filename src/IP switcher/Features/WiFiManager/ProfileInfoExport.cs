using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
using NativeWifi;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    public class ProfileInfoExport
    {
        public string Version { get; set; }

        private List<ProfileInfoExportItem> profiles = new List<ProfileInfoExportItem>();
        public List<ProfileInfoExportItem> Profiles
        {
            get { return profiles; }
            set { profiles = value; }
        }
    }

    public class ProfileInfoExportItem
    {
        public string ProfileName { get; set; }
        public string Profile { get; set; }
        public Wlan.WlanProfileFlags Flags { get; set; }
    }

    public static class ProfileInfoExportExtension
    {
        public static void ReadFromFile(InterfaceModel interFace)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = Resources.ProfileInfoLoc.ExportFilter,
                    CheckPathExists = true,
                    AddExtension = true,
                    FileName = Resources.ProfileInfoLoc.ExportDefaultFilename
                };

            if (!dialog.ShowDialog() ?? false)
                return;

            var reader = new System.Xml.Serialization.XmlSerializer(typeof(ProfileInfoExport));

            var importedProfiles = new ProfileInfoExport();
            try
            {
                if (System.IO.File.Exists(dialog.FileName))
                {
                    using (var file = new System.IO.StreamReader(dialog.FileName))
                    {
                        importedProfiles = (ProfileInfoExport)reader.Deserialize(file);
                    }
                }

                var existingProfiles = interFace.GetProfiles();
                foreach (var profile in importedProfiles.Profiles)
                {
                    if (!existingProfiles.Contains(profile.ProfileName))
                        interFace.interFace.SetProfile(profile.Flags, profile.Profile, false);
                }
            }
            catch (Exception ex)
            {
                Show.Message(string.Format(Resources.ProfileInfoLoc.ErrorImportingLocations, Environment.NewLine, dialog.FileName, ex.Message));
                return;
            }
        }

        public static void WriteToFile(InterfaceModel interFace)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = Resources.ProfileInfoLoc.ExportFilter,
                    CheckPathExists = true,
                    AddExtension = true,
                    FileName = Resources.ProfileInfoLoc.ExportDefaultFilename
                };

            if (!dialog.ShowDialog() ?? false)
                return;

            var writer = new System.Xml.Serialization.XmlSerializer(typeof(ProfileInfoExport));
            var profiles = interFace.GetProfileInfos();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dialog.FileName))
            {
                writer.Serialize(file, new ProfileInfoExport
                {
                    Profiles = profiles.Select(x => new ProfileInfoExportItem
                    {
                        ProfileName = x.profileName,
                        Profile = interFace.GetProfileXml(x.profileName),
                        Flags = x.profileFlags
                    }).ToList(),
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
            }
        }
    }
}
