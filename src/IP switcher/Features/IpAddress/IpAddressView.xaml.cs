using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Deucalion.IP_Switcher.Features.IpAddress
{
    /// <summary>
    /// Interaction logic for IpAddressView.xaml
    /// </summary>
    [ContentProperty("Value")]
    public partial class IpAddressView : UserControl
    {
        public static DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(string),
            typeof(IpAddressView),
            new FrameworkPropertyMetadata("", (sender, e) =>
            {
                var tt = (IpAddressView)sender;
                tt.Parse((string)e.NewValue);
            }));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        public IpAddressView()
        {
            InitializeComponent();
        }

        private string Parse(String text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] splitted = text.Split('.');

            if (splitted.Length == 4)
            {
                Field1.Text = splitted[0];
                Field2.Text = splitted[1];
                Field3.Text = splitted[2];
                Field4.Text = splitted[3];

                return text;
            }

            return string.Empty;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isNumPadNumeric = (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
            bool isNumeric = (e.Key >= Key.D0 && e.Key <= Key.D9);

            if (!isNumeric && !isNumPadNumeric && e.Key != Key.Delete && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Tab)
            {
                e.Handled = true;
                return;
            }

            if ((e.Key == Key.Right && (sender as TextBox).CaretIndex == (sender as TextBox).Text.Length))
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
            TextBox tbx = sender as TextBox;

            byte value = 0;
            if (!string.IsNullOrEmpty(tbx.Text) && !Byte.TryParse(tbx.Text, out value))
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

        private void Field_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
