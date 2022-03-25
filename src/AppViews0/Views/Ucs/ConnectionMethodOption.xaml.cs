using Lucky.Vms;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lucky.Views.Ucs
{
    /// <summary>
    /// Interaction logic for MiningProxyOption.xaml
    /// </summary>
    public partial class ConnectionMethodOption : UserControl
    {
        public MinerProfileViewModel Vm { get; private set; }

        public ConnectionMethodOption()
        {
            this.Vm = AppRoot.MinerProfileVm;
            this.DataContext = AppRoot.MinerProfileVm;
            InitializeComponent();
        }
    }
}
