﻿using System;
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

namespace TTech.IP_Switcher.Features.IpSwitcher
{
    /// <summary>
    /// Interaction logic for IpSwitcherView.xaml
    /// </summary>
    public partial class IpSwitcherView : UserControl
    {
        public IpSwitcherView()
        {
            InitializeComponent();

            var mainViewModel = MainGrid.DataContext as IpSwitcherViewModel;
            if (mainViewModel != null)
                mainViewModel.Owner = this;
        }
    }
}
