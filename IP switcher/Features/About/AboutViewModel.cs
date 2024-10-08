using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        private readonly string latestVersionApi = "https://api.github.com/repos/olathunberg/IP-switcher/releases/latest";
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

        private async void GetLatestVersion()
        {
            var newVersion = await GetVersionFromGitHub();
            if (newVersion.Major == 0 && newVersion.Minor == 0)
                LatestVersion = Resources.AboutViewModelLoc.LatestVersion_Error;
            else
                LatestVersion = newVersion.ToString();
        }

        private async Task<Version> GetVersionFromGitHub()
        {
            return await Task.Run(() =>
            {
                //try
                //{
                //    var request = WebRequest.Create(latestVersionApi) as HttpWebRequest;
                //    request.UserAgent = "curl";
                //    request.Method = "GET";
                //    request.Accept = "application/vnd.github.v3+json";

                //    string releaseJon;
                //    try
                //    {
                //        using (var response = request.GetResponse())
                //        using (var reader = new StreamReader(response.GetResponseStream()))
                //        {
                //            releaseJon = reader.ReadToEnd();
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        releaseJon = null;
                //    }

                //    var jss = new JavaScriptSerializer();
                //    jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

                //    dynamic release = jss.Deserialize(releaseJon, typeof(object)) as dynamic;

                //    return new Version(release.tag_name);
                //}
                //catch
                //{
                    return new Version(0, 0);
                //}
            });
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
