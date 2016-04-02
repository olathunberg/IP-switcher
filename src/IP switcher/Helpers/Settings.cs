using System;
using System.Collections.Generic;
using System.Reflection;
using Deucalion.IP_Switcher.Features.IpSwitcher.Location;
using Deucalion.IP_Switcher.Helpers.ShowWindow;

namespace Deucalion.IP_Switcher
{
    public class Settings
    {
        public Settings()
        {
            Locations = new List<Location>();
        }

        #region Public Properties
        private static Settings defaultInstance = LoadCurrent();
        public static Settings Default
        {
            get { return defaultInstance; }
        }

        public string Version { get; set; }

        public List<Location> Locations { get; set; }
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        #endregion

        #region Methods
        internal uint GetNextID()
        {
            uint Result = 0;

            foreach (var item in Locations)
            {
                if (item.ID > Result)
                    Result = item.ID;
            }

            return Result + 1;
        }

        private static string GetFilePath()
        {
            var Path = string.Format(@"{0}\Deucalion\IP switcher", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create));

            if (!System.IO.Directory.Exists(Path))
                System.IO.Directory.CreateDirectory(Path);

            return Path + @"\Settings.xml";

        }

        internal static void Save()
        {
            defaultInstance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var writer = new System.Xml.Serialization.XmlSerializer(defaultInstance.GetType());

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFilePath()))
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
                    using (System.IO.StreamReader file = new System.IO.StreamReader(GetFilePath()))
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
        #endregion

        #region Events
        public event EventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        protected virtual void OnPropertyChanged(EventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Event Handlers
        #endregion
    }
}
