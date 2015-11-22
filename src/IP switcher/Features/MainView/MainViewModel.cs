using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Deucalion.IP_Switcher.Features.About;
using Deucalion.IP_Switcher.Features.MainView.Resources;
using Deucalion.IP_Switcher.Helpers.ShowWindow;

namespace Deucalion.IP_Switcher.Features.MainView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string title;
        private bool isEnabled = true;
        private bool effect = false;
        private System.Windows.Window owner;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Title = String.Format("{0} v{1} - {2} ({3})",
                    assembly.Name,
                    assembly.Version.ToString(3),
                    Copyright,
                    Company);

            if (!GetDotNetVersions.InstalledDotNetVersions().Any(x => x >= new Version(4, 5)))
                Show.Message(MainViewModelLoc.IncorrectDotNetVersion_Message, MainViewModelLoc.IncorrectDotNetVersion_Caption);

            showAboutCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    Show.Dialog<AboutView>();

                    Effect = false;
                }, () => true);
        }
        #endregion

        #region Public Properties
        public string Copyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length > 0)
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                else
                    return string.Empty;
            }
        }

        public string Company
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length > 0)
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                else
                    return string.Empty;
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                    return;

                isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool Effect
        {
            get { return effect; }
            set
            {
                if (effect == value)
                    return;

                effect = value;

                NotifyPropertyChanged();
            }
        }

        public System.Windows.Window Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                owner = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Private / Protected
        #endregion

        #region Methods

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Commands
        private readonly RelayCommand showAboutCommand;
        public ICommand ShowAbout { get { return showAboutCommand; } }
        #endregion
    }
}
