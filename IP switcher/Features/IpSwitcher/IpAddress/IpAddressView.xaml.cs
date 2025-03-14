using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace TTech.IP_Switcher.Features.IpSwitcher.IpAddress;

/// <summary>
/// Interaction logic for IpAddressView.xaml
/// </summary>
[ContentProperty("Value")]
public partial class IpAddressView : UserControl
{
    public IpAddressView()
    {
        InitializeComponent();
    }

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
     nameof(Value),
     typeof(string),
     typeof(IpAddressView),
     new FrameworkPropertyMetadata("", (sender, e) =>
     {
         var tt = (IpAddressView)sender;
         tt.Parse((string)e.NewValue);
     }));


    private void Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        string[] splittedText = text.Split('.');

        if (splittedText.Length == 4)
        {
            Field1.Text = splittedText[0];
            Field2.Text = splittedText[1];
            Field3.Text = splittedText[2];
            Field4.Text = splittedText[3];
        }
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var isNumPadNumeric = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
        var isNumeric = e.Key >= Key.D0 && e.Key <= Key.D9;

        Key[] keysToFilter = [Key.Delete, Key.Back, Key.Left, Key.Right, Key.Tab];
        if (!isNumeric && !isNumPadNumeric && !keysToFilter.Contains(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && (sender as TextBox).CaretIndex == (sender as TextBox).Text.Length)
        {
            if (sender == Field1)
                Field2.Focus();
            else if (sender == Field2)
                Field3.Focus();
            else if (sender == Field3)
                Field4.Focus();

            e.Handled = true;
        }
        else if (e.Key == Key.Left && (sender as TextBox).CaretIndex == 0)
        {
            if (sender == Field4)
                Field3.Focus();
            else if (sender == Field3)
                Field2.Focus();
            else if (sender == Field2)
                Field1.Focus();

            e.Handled = true;
        }
        else if (e.Key == Key.OemMinus)
            e.Handled = true;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var tbx = sender as TextBox;

        byte value = 0;
        if (!string.IsNullOrEmpty(tbx.Text) && !byte.TryParse(tbx.Text, out value))
        {
            tbx.Text = "255";

            if (sender == Field1)
                Field2.Focus();
            else if (sender == Field2)
                Field3.Focus();
            else if (sender == Field3)
                Field4.Focus();
        }
        else if (tbx.Text.Length == 3)
        {
            if (sender == Field1)
                Field2.Focus();
            else if (sender == Field2)
                Field3.Focus();
            else if (sender == Field3)
                Field4.Focus();
        }

        SetValue(ValueProperty, string.Format("{0}.{1}.{2}.{3}", Field1.Text, Field2.Text, Field3.Text, Field4.Text));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "<Pending>")]
    private void Field_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        (sender as TextBox).SelectAll();
    }
}
