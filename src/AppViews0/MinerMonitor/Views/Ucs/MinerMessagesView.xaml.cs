using Lucky.MinerMonitor;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerMessagesView : UserControl
    {
        private MinerMonitorRoot.MinerTweakMessagesViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweakMessagesVm;
            }
        }

        public MinerMessagesView()
        {
            this.DataContext = MinerMonitorRoot.MinerTweakMessagesVm;
            InitializeComponent();
        }
    }
}
