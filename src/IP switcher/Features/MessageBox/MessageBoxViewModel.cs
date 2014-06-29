using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Deucalion.IP_Switcher.Features.MessageBox
{
    public class MessageBoxViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Window owner;
        #endregion

        #region Constructors
        public MessageBoxViewModel()
        { }
        #endregion

        #region Public Properties
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

        public string Caption { get; set; }

        public string Content { get; set; }

        public bool ShowOkButton { get; set; }

        public bool ShowCancelButton { get; set; }

        public bool OkIsCancel { get; set; }
        #endregion

        #region Private / Protected
        #endregion

        #region Methods
        public bool Show(System.Windows.Window owner, string caption, string content, bool ShowCancel = true)
        {
            Caption = caption;
            Content = content;
            ShowOkButton = true;
            ShowCancelButton = false;
            OkIsCancel = !ShowCancelButton;
            var dialog = new MessageBoxView(this)
                 {
                     WindowStartupLocation = WindowStartupLocation.CenterOwner,
                     Owner = owner,
                 }.ShowDialog();

            if (dialog.HasValue)
                return dialog.Value;
            else
                return false;
        }

        public bool Show(string content, string caption, bool ShowCancel = true)
        {
            Caption = caption;
            Content = content;
            ShowOkButton = true;
            ShowCancelButton = false;
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
        #endregion
    }
}
