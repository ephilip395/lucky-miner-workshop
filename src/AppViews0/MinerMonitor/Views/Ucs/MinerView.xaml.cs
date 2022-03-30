using Lucky.MinerMonitor.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerView : UserControl
    {
        public MinerView()
        {
            InitializeComponent();
            
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MinerViewModel vm = (MinerViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(null);
            e.Handled = true;
        }
    }
}
