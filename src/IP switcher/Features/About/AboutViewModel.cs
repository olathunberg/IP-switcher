using Deucalion.IP_Switcher.Helpers.ShowWindow;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Deucalion.IP_Switcher.Features.About
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Window owner;
        private string latestVersion;
        private readonly string codePlexLink = "https://ipswitcher.codeplex.com/";
        #endregion

        #region Constructors
        public AboutViewModel()
        {
            GetLatestVersion();
        }
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

        public string ProjectCaption
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public string Version
        {
            get
            {
                return string.Format("{0} {1}", Resources.AboutViewModelLoc.Version, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            }
        }

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

        public string CodePlexUrl
        {
            get { return "ipswitcher.codeplex.com"; }
        }
        #endregion

        #region Private / Protected
        #endregion

        #region Methods
        private void OpenWebPage()
        {
            try
            {
                System.Diagnostics.Process.Start(codePlexLink);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    Show.Message( noBrowser.Message);
            }
            catch (System.Exception other)
            {
                Show.Message(other.Message);
            }
        }

        private async void GetLatestVersion()
        {
            var newVersion = await GetVersionFromCodePlex();
            if (newVersion.Major == 0 && newVersion.Minor == 0)
                LatestVersion = Resources.AboutViewModelLoc.LatestVersion_Error;
            else
                LatestVersion = newVersion.ToString();
        }

        /// <summary>
        /// Temporary solution, can´t rely on CodePlex keeping their HTML intact
        /// </summary>
        /// <returns></returns>
        private async Task<Version> GetVersionFromCodePlex()
        {
            try
            {
                string webPageString = await new WebClient().DownloadStringTaskAsync(new Uri(codePlexLink));

                // Find substring marking header of current version
                var index = webPageString.IndexOf("<th><span class=\"rating_header\">current</span></th>", StringComparison.Ordinal);

                // Extract first <td> tag, which contains name of current version
                index = webPageString.IndexOf("<td>", index, StringComparison.Ordinal) + 4;
                var index2 = webPageString.IndexOf("</td>", index, StringComparison.Ordinal) - 4;
                var productString = webPageString.Substring(index, index2 - index).Trim();

                string versionNumber = new string(productString.Where(x => Char.IsNumber(x) || Char.IsPunctuation(x)).ToArray());

                return new Version(versionNumber);
            }
            catch
            {
                return new Version(0, 0);
            }
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
        public ICommand WebPageLink
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OpenWebPage();
                }, () => true);
            }
        }
        #endregion
    }
}
