using System;
using System.Collections.Generic;
using System.Reflection;
using TTech.IP_Switcher.Features.IpSwitcher.Location.Resources;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.IpSwitcher.Location
{
    public class LocationExport
    {
        public string Version { get; set; }

        private List<Location> _Locations = new List<Location>();
        public List<Location> Locations
        {
            get { return _Locations; }
            set { _Locations = value; }
        }
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
                return null;

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

                Settings.Default.Locations.AddRange(importedLocations.Locations);
                Settings.Save();
                return Settings.Default.Locations;
            }
            catch (Exception ex)
            {
                Show.Message(String.Format(LocationModelLoc.ErrorImportingLocations, Environment.NewLine, dialog.FileName, ex.Message));
                return null;
            }
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
