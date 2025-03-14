using System.Windows;

namespace TTech.IP_Switcher.Features.InputBox;

/// <summary>
/// Interaction logic for InputBox.xaml
/// </summary>
public partial class InputBoxView : Window
{
    public string Result { get; set; }

    public InputBoxView()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        tbDescription.Focus();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        Result = tbDescription.Text;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
