using Deucalion.IP_Switcher.Features.About;
using Deucalion.IP_Switcher.Features.MainView.Resources;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

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
            Title = String.Format("{0} v{1} - Ola Thunberg 2012-2015",
                    assembly.Name,
                    assembly.Version.ToString(3));

            if (!GetDotNetVersions.InstalledDotNetVersions().Any(x => x >= new Version(4, 5)))
                Show.Message(MainViewModelLoc.IncorrectDotNetVersion_Message, MainViewModelLoc.IncorrectDotNetVersion_Caption);

            showAboutCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    Show.Dialog<AboutView>();

                    Effect = false;
                }, () => true);

            // Experiment, pre JIT
            //var jitter = new Thread(() =>
            //    {
            //        foreach (var type in Assembly.Load("IP switcher").GetTypes())
            //        {
            //            foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly |
            //                                                   BindingFlags.NonPublic |
            //                                                   BindingFlags.Public | 
            //                                                   BindingFlags.Instance |
            //                                                   BindingFlags.Static))
            //                if ((method.Attributes & MethodAttributes.Abstract) != MethodAttributes.Abstract && !method.ContainsGenericParameters)
            //                    System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
            //        }
            //    });
            //jitter.Priority = ThreadPriority.Lowest;
            //jitter.Start();
        }
        #endregion

        #region Public Properties
        public string Title { get { return title; } set { title = value; NotifyPropertyChanged(); } }

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
        private RelayCommand showAboutCommand;
        public ICommand ShowAbout { get { return showAboutCommand; } }
        #endregion
    }
}
