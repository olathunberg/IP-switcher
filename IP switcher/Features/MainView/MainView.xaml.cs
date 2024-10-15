using System.Windows;

namespace TTech.IP_Switcher.Features.MainView
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Title = ((MainViewModel)MainGrid.DataContext).Title;
        }
    }
}
