using System.Windows;

namespace TTech.IP_Switcher.Features.About;

/// <summary>
/// Interaction logic for AboutView.xaml
/// </summary>
public partial class AboutView : Window
{
    public AboutView()
    {
        InitializeComponent();

        ((AboutViewModel)MainGrid.DataContext).Owner = this;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
