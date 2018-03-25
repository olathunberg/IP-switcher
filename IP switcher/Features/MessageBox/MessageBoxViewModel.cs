﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TTech.IP_Switcher.Features.MessageBox
{
    public class MessageBoxViewModel : INotifyPropertyChanged
    {
        #region Fields
        private Window owner;
        #endregion

        #region Constructors
        #endregion

        #region Public Properties
        public Window Owner
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

        public string Caption { get; set; }

        public string Content { get; set; }

        public bool ShowOkButton { get; set; }

        public bool ShowCancelButton { get; set; }

        public bool OkIsCancel { get; set; }
        #endregion

        #region Private / Protected
        #endregion

        #region Methods
        public bool Show(Window owner, string caption, string content, bool showCancel = false)
        {
            Caption = caption;
            Content = content;
            ShowOkButton = true;
            ShowCancelButton = showCancel;
            OkIsCancel = !ShowCancelButton;
            var dialog = new MessageBoxView(this)
                 {
                     WindowStartupLocation = WindowStartupLocation.CenterOwner,
                     Owner = owner
                 }.ShowDialog();

            if (dialog.HasValue)
                return dialog.Value;

            return false;
        }

        public bool Show(string content, string caption, bool ShowCancel = false)
        {
            Caption = caption;
            Content = content;
            ShowOkButton = true;
            ShowCancelButton = ShowCancel;
            OkIsCancel = !ShowCancelButton;
            var dialog = new MessageBoxView(this).ShowDialog();

            if (dialog.HasValue)
                return dialog.Value;
            else
                return false;
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Commands
        #endregion
    }
}
