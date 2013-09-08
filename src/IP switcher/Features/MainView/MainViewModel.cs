﻿using Deucalion.IP_Switcher.Features.About;
using Deucalion.IP_Switcher.Features.MainView.Resources;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Deucalion.IP_Switcher.Features.MainView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Window _Owner;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            Title = String.Format("{0} v{1} - Ola Thunberg 2012-2013",
                    assembly.Name,
                    assembly.Version.ToString(3));

            if (!GetDotNetVersions.InstalledDotNetVersions().Any(x => x >= new Version(4, 5)))
                Show.Message(MainViewModelLoc.IncorrectDotNetVersion_Message, MainViewModelLoc.IncorrectDotNetVersion_Caption);

            showAboutCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    Show.Dialog<AboutView>();

                    Effect = false;
                },()=> true);
        }
        #endregion

        #region Public Properties
        private string title;
        public string Title { get { return title; } set { title = value; NotifyPropertyChanged(); } }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value; NotifyPropertyChanged();
            }
        }

        private bool effect = false;
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
            get { return _Owner; }
            set
            {
                if (_Owner == value)
                    return;

                _Owner = value;

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
        private RelayCommand showAboutCommand;
        public ICommand ShowAbout { get { return showAboutCommand; } }
        #endregion
    }
}
