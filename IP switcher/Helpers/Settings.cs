using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Helpers.Logging;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher;

public class Settings
{
    private static readonly IAppLogger AppLogger = Helpers.Logging.AppLogger.Instance;
    private static Settings defaultInstance = LoadCurrent();
    public static Settings Default => defaultInstance;

    public string Version { get; set; }

    public List<Location> Locations { get; set; } = [];

    internal uint GetNextID()
    {
        uint Result = 0;

        foreach (var itemID in Locations.Select(x => x.ID))
        {
            if (itemID > Result)
                Result = itemID;
        }

        return Result + 1;
    }

    private static string GetFilePath()
    {
        var settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
            "TTech",
            "IP switcher");

        Directory.CreateDirectory(settingsDirectory);

        return Path.Combine(settingsDirectory, "Settings.xml");

    }

    internal static void Save()
    {
        defaultInstance ??= new Settings();
        defaultInstance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        var targetPath = GetFilePath();
        var tempPath = targetPath + ".tmp";
        var backupPath = targetPath + ".bak";

        try
        {
            var writer = new System.Xml.Serialization.XmlSerializer(defaultInstance.GetType());

            using (StreamWriter file = new(tempPath))
            {
                writer.Serialize(file, defaultInstance);
            }

            if (File.Exists(targetPath))
            {
                File.Replace(tempPath, targetPath, backupPath, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempPath, targetPath);
            }

            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError("Settings.Save", "Failed to persist settings to disk.", ex);
            Show.Message("Settings could not be saved.");

            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
                // Ignore cleanup failures.
            }
        }
    }

    internal static void Reload()
    {
        defaultInstance = LoadCurrent();
    }

    internal static Settings LoadCurrent()
    {
        var reader = new System.Xml.Serialization.XmlSerializer(typeof(Settings));

        var newSettings = new Settings();

        try
        {
            var filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                using (StreamReader file = new(filePath))
                {
                    var loadedSettings = reader.Deserialize(file) as Settings;
                    if (loadedSettings != null)
                    {
                        newSettings = loadedSettings;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError("Settings.LoadCurrent", "Failed to load settings from disk.", ex);
            Show.Message("Settings could not be loaded. Default settings will be used.");
        }

        newSettings.Locations ??= [];

        newSettings.PropertyChanged += (sender, e) => Settings.Save();

        return newSettings;
    }

    public event EventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event.
    /// </summary>
    protected virtual void OnPropertyChanged(EventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}
