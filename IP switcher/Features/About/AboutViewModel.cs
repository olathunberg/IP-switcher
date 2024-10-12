using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.About
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Window owner;
        private string latestVersion;
        private readonly string webLink = "https://github.com/olathunberg/IP-switcher";
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

        public string ProjectCaption => Assembly.GetExecutingAssembly().GetName().Name;

        public string Version => string.Format("{0} {1}", Resources.AboutViewModelLoc.Version, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

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

        public string LatestVersion
        {
            get
            {
                if (latestVersion == null)
                    return Resources.AboutViewModelLoc.LatestVersion_Searching;
                else
                    return string.Format("{0} {1}", Resources.AboutViewModelLoc.LatestVersion, latestVersion);
            }
            set
            {
                latestVersion = value;
                NotifyPropertyChanged();
            }
        }

        public string WebUrl => $"{ProjectCaption} on GitHub";
        #endregion

        #region Private / Protected
        #endregion

        #region Methods
        private void OpenWebPage()
        {
            try
            {
                System.Diagnostics.Process.Start(webLink);
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    Show.Message(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                Show.Message(other.Message);
            }
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
        public ICommand WebPageLink => new RelayCommand(OpenWebPage, () => true);
        #endregion
    }
}
