using Lucky.MinerMonitor.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerTweak : UserControl
    {
        public MinerTweak()
        {
            InitializeComponent();
            
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MinerTweakViewModel vm = (MinerTweakViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(null);
            e.Handled = true;
        }
    }
}
