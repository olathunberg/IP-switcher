using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Wireless_Network_Manager;

namespace Deucalion.IP_Switcher.Features.MainView
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="frmMain" /> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            ((MainViewModel)MainGrid.DataContext).Owner = this;
            Title = ((MainViewModel)MainGrid.DataContext).Title;
            IpSwitcherView.Focus();
        }
        #endregion

        private void IpButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IpSwitcherView == null)
                return;

            IpSwitcherView.Visibility = System.Windows.Visibility.Visible;
            Storyboard storyboard = Resources["SlideInFromLeft"] as Storyboard;
            storyboard.Begin(IpSwitcherView);
            IpSwitcherView.Focus();
        }

        private void IpButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IpSwitcherView == null)
                return;

            Storyboard storyboard = Resources["FadeOut"] as Storyboard;
            storyboard.Begin(IpSwitcherView);
            IpSwitcherView.Visibility = System.Windows.Visibility.Hidden;
        }

        private void WiFiButton_Checked(object sender, RoutedEventArgs e)
        {
            if (WIFIManagerView == null)
                return;

            WIFIManagerView.Visibility = System.Windows.Visibility.Visible;
            Storyboard storyboard = Resources["SlideInFromRight"] as Storyboard;
            storyboard.Begin(WIFIManagerView);
            WIFIManagerView.Focus();
        }

        private void WiFiButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (WIFIManagerView == null)
                return;

            Storyboard storyboard = Resources["FadeOut"] as Storyboard;
            storyboard.Begin(WIFIManagerView);
            WIFIManagerView.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
