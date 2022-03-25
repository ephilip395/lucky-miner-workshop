using Lucky.MinerMonitor.Vms;

using System;
using System.Windows;


namespace Lucky.MinerMonitor.Views
{
    /// <summary>
    /// Interaction logic for MinerDetailsWindow.xaml
    /// </summary>
    public partial class MinerDetailsWindow : Window
    {
        public MinerTweaksWindowViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweaksWindowVm;
            }
        }

        public MinerDetailsWindow()
        {
            DataContext = Vm;
            InitializeComponent();
        }
    }
}
