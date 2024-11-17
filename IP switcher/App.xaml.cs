using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace TTech.IP_Switcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string Unique = "AB7DE433-31E1-4114-A247-D780C349040B";

        [STAThread]
        public static void Main()
        {
            using (var mutex = new Mutex(false, Unique))
            {
                if (!mutex.WaitOne(0))
                {
                    MessageBox.Show("Another instance is already running");
                    return;
                }
    
                FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

                var application = new App();

                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                application.InitializeComponent();

                application.Run();
            }
        }
    }
}
