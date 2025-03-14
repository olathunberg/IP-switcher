using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TTech.IP_Switcher.Features.MessageBox;

public class MessageBoxViewModel : INotifyPropertyChanged
{

    private Window owner;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Caption { get; set; }

    public string Content { get; set; }

    public bool OkIsCancel { get; set; }

    public Window Owner
    {
        get => owner;
        set
        {
            if (owner == value)
                return;

            owner = value;

            NotifyPropertyChanged();
        }
    }
    
    public bool ShowCancelButton { get; set; }
    public bool ShowOkButton { get; set; }

    public bool Show(Window owner, string caption, string content, bool showCancel = false)
    {
        Caption = caption;
        Content = content;
        ShowOkButton = true;
        ShowCancelButton = showCancel;
        OkIsCancel = !ShowCancelButton;
        var dialog = new MessageBoxView(this)
             {
                 WindowStartupLocation = WindowStartupLocation.CenterOwner,
                 Owner = owner
             }.ShowDialog();

        return dialog ?? false;
    }

    public bool Show(string content, string caption, bool ShowCancel = false)
    {
        Caption = caption;
        Content = content;
        ShowOkButton = true;
        ShowCancelButton = ShowCancel;
        OkIsCancel = !ShowCancelButton;
        var dialog = new MessageBoxView(this).ShowDialog();

        return dialog ?? false;
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
