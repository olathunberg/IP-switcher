using Deucalion.IP_Switcher.Classes;
using Deucalion.IP_Switcher.ViewModels;
using System.Windows;
using System.Windows.Media.Effects;

namespace Deucalion.IP_Switcher
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="frmMain" /> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            ((MainViewModel)MainGrid.DataContext).Owner = this;
            Title = ((MainViewModel)MainGrid.DataContext).Title;
        }
        #endregion
    }
}
