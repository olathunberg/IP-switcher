using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Deucalion.IP_Switcher.Features.IpSwitcher
{
    /// <summary>
    /// Interaction logic for IpSwitcherView.xaml
    /// </summary>
    public partial class IpSwitcherView : UserControl
    {
        public IpSwitcherView()
        {
            InitializeComponent();
            ((IpSwitcherViewModel)MainGrid.DataContext).Owner = this;
        }
    }
}
