using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TTech.IP_Switcher.Helpers;

namespace TTech.IP_Switcher.Features.MainView;

public class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly Lock errorLock = new();
    private readonly List<ErrorEntry> errorEntries = [];
    private readonly Dictionary<long, CancellationTokenSource> errorExpirations = [];
    private readonly Dispatcher dispatcher;
    private bool isEnabled = true;
    private bool isDisposed;
    private long nextErrorId;
    private string title;

    public MainViewModel()
    {
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        var assembly = Assembly.GetExecutingAssembly().GetName();
        Title = string.Format("{0} v{1} - {2}",
                assembly.Name,
                assembly.Version.ToString(3),
                Company);

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
        get
        {
            lock (errorLock)
            {
                return string.Join(Environment.NewLine, errorEntries.Select(x => x.Text));
            }
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value) || isDisposed)
                return;

            if (!dispatcher.CheckAccess())
            {
                _ = dispatcher.InvokeAsync(() => AddError(value));
                return;
            }

            AddError(value);
        }
    }

    public bool HasErrorText => !string.IsNullOrEmpty(ErrorText);

    public void Dispose()
    {
        lock (errorLock)
        {
            if (isDisposed)
                return;

            isDisposed = true;
            foreach (var expiration in errorExpirations.Values)
                expiration.Cancel();
            errorExpirations.Clear();
            errorEntries.Clear();

            GC.SuppressFinalize(this);
        }
    }

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

    private void AddError(string value)
    {
        if (isDisposed)
            return;

        ErrorEntry entry;
        lock (errorLock)
        {
            if (isDisposed)
                return;

            entry = new ErrorEntry(++nextErrorId, value);
            errorEntries.Add(entry);
            errorExpirations[entry.Id] = new CancellationTokenSource();
        }

        NotifyPropertyChanged(nameof(ErrorText));
        NotifyPropertyChanged(nameof(HasErrorText));
        _ = ExpireErrorAsync(entry);
    }

    private async Task ExpireErrorAsync(ErrorEntry entry)
    {
        CancellationTokenSource expiration;
        lock (errorLock)
        {
            if (!errorExpirations.TryGetValue(entry.Id, out expiration))
                return;
        }

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), expiration.Token);
            await dispatcher.InvokeAsync(() => RemoveError(entry));
        }
        catch
        {
            // Intentionally left blank
        }
        finally
        {
            lock (errorLock)
            {
                errorExpirations.Remove(entry.Id);
            }

            expiration.Dispose();
        }
    }

    private void RemoveError(ErrorEntry entry)
    {
        lock (errorLock)
        {
            if (isDisposed || !errorEntries.Remove(entry))
                return;
        }

        NotifyPropertyChanged(nameof(ErrorText));
        NotifyPropertyChanged(nameof(HasErrorText));
    }

    private sealed record ErrorEntry(long Id, string Text);
}
