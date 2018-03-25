using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Microsoft.Shell;

namespace TTech.IP_Switcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "TTech-IPSwitcher";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                application.InitializeComponent();

                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            this.MainWindow.Activate();

            return true;
        }

        #endregion
    }
}
