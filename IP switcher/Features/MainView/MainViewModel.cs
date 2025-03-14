using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TTech.IP_Switcher.Helpers;

namespace TTech.IP_Switcher.Features.MainView;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly List<string> errorText;
    private bool isEnabled = true;
    private string title;

    public MainViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly().GetName();
        Title = string.Format("{0} v{1} - {2}",
                assembly.Name,
                assembly.Version.ToString(3),
                Company);

        errorText = [];
        SimpleMessenger.Default.Register<string>("ErrorText", x => ErrorText = x);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public static string Company
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

    public static string Copyright
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

    public string ErrorText
    {
        get => string.Join(Environment.NewLine, errorText);
        set
        {
            errorText.Add(value);
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(HasErrorText));

            Task.Delay(5000).ContinueWith(ante =>
            {
                if (errorText.Count > 0)
                    errorText.Remove(value);
                NotifyPropertyChanged(nameof(ErrorText));
                NotifyPropertyChanged(nameof(HasErrorText));
            });
        }
    }

    public bool HasErrorText => !string.IsNullOrEmpty(ErrorText);

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled == value)
                return;

            isEnabled = value;
            NotifyPropertyChanged();
        }
    }

    public string Title
    {
        get => title;
        set
        {
            title = value;
            NotifyPropertyChanged();
        }
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
