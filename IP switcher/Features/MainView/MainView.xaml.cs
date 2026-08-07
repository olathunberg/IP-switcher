using System;
using System.Windows;

namespace TTech.IP_Switcher.Features.MainView;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        Title = ((MainViewModel)MainGrid.DataContext).Title;
        Closed += MainView_Closed;
    }

    private void MainView_Closed(object sender, EventArgs e)
    {
        ((MainViewModel)MainGrid.DataContext).Dispose();
    }
}
