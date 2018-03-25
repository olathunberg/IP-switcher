using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TTech.IP_Switcher.Features.About;
using TTech.IP_Switcher.Features.MainView.Resources;
using TTech.IP_Switcher.Helpers;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.MainView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string title;
        private bool isEnabled = true;
        private bool effect;
        private readonly List<string> errortext;
        private System.Windows.Window owner;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            Title = string.Format("{0} v{1} - {2}",
                    assembly.Name,
                    assembly.Version.ToString(3),
                    Company);

            if (!GetDotNetVersions.InstalledDotNetVersions().Any(x => x >= new Version(4, 5)))
                Show.Message(MainViewModelLoc.IncorrectDotNetVersion_Message, MainViewModelLoc.IncorrectDotNetVersion_Caption);

            showAboutCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    Show.Dialog<AboutView>();

                    Effect = false;
                }, () => true);

            errortext = new List<string>();
            SimpleMessenger.Default.Register<string>("ErrorText", x => ErrorText = x);
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


        public string ErrorText
        {
            get { return string.Join(Environment.NewLine, errortext); }
            set
            {
                errortext.Add(value);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HasErrortext));

                Task.Delay(5000).ContinueWith(ante =>
                {
                    if (errortext.Count > 0)
                        errortext.Remove(value);
                    NotifyPropertyChanged(nameof(ErrorText));
                    NotifyPropertyChanged(nameof(HasErrortext));
                });
            }
        }

        public bool HasErrortext { get { return !string.IsNullOrEmpty(ErrorText); } }
        #endregion

        #region Private / Protected
        #endregion

        #region Methods

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
