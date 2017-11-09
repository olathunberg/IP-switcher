using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TTech.IP_Switcher.Features.IpSwitcher.Location.Resources;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.IpSwitcher.Location
{
    public class LocationExport
    {
        public string Version { get; set; }

        public List<Location> Locations { get; set; }
    }

    public static class LocationExportExtension
    {
        public static List<Location> ReadFromFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = LocationModelLoc.ExportFilter,
                CheckPathExists = true,
                AddExtension = true,
                FileName = LocationModelLoc.ExportDefaultFilename
            };

            if (!dialog.ShowDialog() ?? false)
                return Settings.Default.Locations;

            var reader = new System.Xml.Serialization.XmlSerializer(typeof(LocationExport));

            var importedLocations = new LocationExport();
            try
            {
                if (System.IO.File.Exists(dialog.FileName))
                {
                    using (var file = new System.IO.StreamReader(dialog.FileName))
                    {
                        importedLocations = (LocationExport)reader.Deserialize(file);
                    }
                }

                foreach (var location in importedLocations.Locations)
                {
                    if (Settings.Default.Locations.Any(x => x.Description == location.Description))
                    {
                        if (Show.Message(string.Format(LocationModelLoc.ImportDuplicateCaption, location.Description), LocationModelLoc.ImportDuplicateBody, AllowCancel: true))
                            Settings.Default.Locations.Add(location);

                        continue;
                    }

                    Settings.Default.Locations.Add(location);
                }

                Settings.Save();
            }
            catch (Exception ex)
            {
                Show.Message(String.Format(LocationModelLoc.ErrorImportingLocations, Environment.NewLine, dialog.FileName, ex.Message));
            }

            return Settings.Default.Locations;
        }

        public static void WriteToFile(List<Location> Locations)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = LocationModelLoc.ExportFilter,
                CheckPathExists = true,
                AddExtension = true,
                FileName = LocationModelLoc.ExportDefaultFilename
            };

            if (!dialog.ShowDialog() ?? false)
                return;

            var writer = new System.Xml.Serialization.XmlSerializer(typeof(LocationExport));

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dialog.FileName))
            {
                writer.Serialize(file, new LocationExport
                {
                    Locations = Locations,
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
            }
        }
    }
}
