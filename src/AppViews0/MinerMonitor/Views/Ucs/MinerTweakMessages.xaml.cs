using Lucky.MinerMonitor;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerTweakMessages : UserControl
    {
        private MinerMonitorRoot.MinerTweakMessagesViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweakMessagesVm;
            }
        }

        public MinerTweakMessages()
        {
            this.DataContext = MinerMonitorRoot.MinerTweakMessagesVm;
            InitializeComponent();
        }
    }
}
