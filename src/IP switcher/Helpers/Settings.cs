using Deucalion.IP_Switcher.Features;
using Deucalion.IP_Switcher.Features.Location;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Deucalion.IP_Switcher
{
    public class Settings
    {
        #region Public Properties
        private static Settings defaultInstance = LoadCurrent();
        public static Settings Default
        {
            get { return defaultInstance; }
        }

        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        private List<Location> _Locations = new List<Location>();
        public List<Location> Locations
        {
            get { return _Locations; }
            set { _Locations = value; }
        }
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
            string Path = String.Format(@"{0}\Deucalion\IP switcher", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create));

            if (!System.IO.Directory.Exists(Path))
                System.IO.Directory.CreateDirectory(Path);

            return Path + @"\Settings.xml";

        }

        internal static void Save()
        {
            defaultInstance.Version =  Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(defaultInstance.GetType());

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
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Settings));

            Settings newSettings = new Settings();

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
               Show.Message(String.Format("Couldn't read settings from file:{0}{1}{0}{0}Exception:{0}{2}", Environment.NewLine, GetFilePath(), ex.Message));
            }

            newSettings.PropertyChanged += (sender, e) => Settings.Save();

            return newSettings;
        }
        #endregion

        #region Events
        private readonly object PropertyChangedEventLock = new object();
        private EventHandler PropertyChangedEvent;

        /// <summary>
        /// Event raised after the <see cref="Text" /> property value has changed.
        /// </summary>
        public event EventHandler PropertyChanged
        {
            add
            {
                lock (PropertyChangedEventLock)
                {
                    PropertyChangedEvent += value;
                }
            }
            remove
            {
                lock (PropertyChangedEventLock)
                {
                    PropertyChangedEvent -= value;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        protected virtual void OnPropertyChanged()
        {
            EventHandler handler = null;

            lock (PropertyChangedEventLock)
            {
                handler = PropertyChangedEvent;

                if (handler == null)
                    return;
            }

            handler(this, new EventArgs());
        }
        #endregion

        #region Event Handlers
        #endregion
    }
}
