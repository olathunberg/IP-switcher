using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher;

public class Settings
{
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
        var Path = string.Format(@"{0}\TTech\IP switcher", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create));

        if (!System.IO.Directory.Exists(Path))
            System.IO.Directory.CreateDirectory(Path);

        return Path + @"\Settings.xml";

    }

    internal static void Save()
    {
        defaultInstance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        var writer = new System.Xml.Serialization.XmlSerializer(defaultInstance.GetType());

        using (System.IO.StreamWriter file = new(GetFilePath()))
        {
            writer.Serialize(file, defaultInstance);
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
            if (System.IO.File.Exists(GetFilePath()))
            {
                using (System.IO.StreamReader file = new(GetFilePath()))
                {
                    newSettings = (Settings)reader.Deserialize(file);
                }
            }
        }
        catch (Exception ex)
        {
            Show.Message(string.Format("Couldn't read settings from file:{0}{1}{0}{0}Exception:{0}{2}", Environment.NewLine, GetFilePath(), ex.Message));
        }

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
