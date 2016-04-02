using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Deucalion.IP_Switcher.Features.MainView
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private TouchPoint touchStart;
        private RadioButton currentlyCheckedButton;

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="MainView" /> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            ((MainViewModel)MainGrid.DataContext).Owner = this;
            Title = ((MainViewModel)MainGrid.DataContext).Title;
            IpSwitcherView.Focus();

            // Get initially selected button
            foreach (var item in ButtonsPanel.Children)
            {
                var button = item as RadioButton;
                if (button != null && (button.IsChecked ?? false))
                    currentlyCheckedButton = button;
            }
        }
        #endregion

        private void IpButton_Checked(object sender, RoutedEventArgs e)
        {
            SlideIn(IpSwitcherView, IpButton);
        }

        private void IpButton_Unchecked(object sender, RoutedEventArgs e)
        {
            FadeOut(IpSwitcherView);
        }

        private void WiFiButton_Checked(object sender, RoutedEventArgs e)
        {
            SlideIn(WiFiNetworksView, WiFiButton);
        }

        private void WiFiButton_Unchecked(object sender, RoutedEventArgs e)
        {
            FadeOut(WiFiNetworksView);
        }

        private void WiFiProfilesButton_Checked(object sender, RoutedEventArgs e)
        {
            SlideIn(WiFiProfilesView, WiFiProfilesButton);
        }

        private void WiFiProfilesButton_Unchecked(object sender, RoutedEventArgs e)
        {
            FadeOut(WiFiProfilesView);
        }

        private void MainViewWindow_TouchDown(object sender, TouchEventArgs e)
        {
            touchStart = e.TouchDevice.GetTouchPoint(MainGrid);
            e.Handled = true;
        }

        private void MainViewWindow_TouchMove(object sender, TouchEventArgs e)
        {
            if (touchStart == null)
                return;

            // Swipe right
            if (e.TouchDevice.GetTouchPoint(MainGrid).Position.X > touchStart.Position.X + 200)
            {
                RadioButton previous = null;
                foreach (var item in ButtonsPanel.Children)
                {
                    var radioButton = item as RadioButton;
                    if (radioButton != null)
                    {
                        if (radioButton.IsChecked ?? false)
                        {
                            if (previous != null)
                                previous.IsChecked = true;
                            break;
                        }
                        previous = radioButton;
                    }
                }

                touchStart = null;
            }
            // Swipe left
            else if (e.TouchDevice.GetTouchPoint(MainGrid).Position.X < touchStart.Position.X - 200)
            {
                bool checkNext = false;
                foreach (var item in ButtonsPanel.Children)
                {
                    var radioButton = item as RadioButton;
                    if (radioButton != null)
                    {
                        if (checkNext)
                        {
                            radioButton.IsChecked = true;
                            break;
                        }
                        if (radioButton.IsChecked ?? false)
                            checkNext = true;
                    }
                }

                touchStart = null;
            }

            e.Handled = true;
        }

        private void SlideIn(UserControl view, RadioButton button)
        {
            if (view == null || button == null)
                return;

            view.Visibility = Visibility.Visible;
            Storyboard storyboard;
            bool foundPrevious = false;

            // Search for checked button,
            // if present before finding previously checked moving left.
            // Otherwise moving right
            foreach (var item in ButtonsPanel.Children)
            {
                var radioButton = item as RadioButton;

                if (radioButton != null && (radioButton.IsChecked ?? false))
                {
                    currentlyCheckedButton = radioButton;
                    break;
                }
                if (radioButton != null && radioButton == currentlyCheckedButton)
                    foundPrevious = true;
            }

            if (foundPrevious)
                storyboard = Resources["SlideInFromRight"] as Storyboard;
            else
                storyboard = Resources["SlideInFromLeft"] as Storyboard;

            storyboard.Begin(view);
            view.Focus();
        }

        private void FadeOut(UserControl sender)
        {
            if (sender == null)
                return;

            var storyboard = Resources["FadeOut"] as Storyboard;
            storyboard.Begin(sender);
            sender.Visibility = Visibility.Hidden;
        }
    }
}
