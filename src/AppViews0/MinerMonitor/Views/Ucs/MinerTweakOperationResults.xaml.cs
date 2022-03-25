using Lucky.MinerMonitor;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerTweakOperationResults : UserControl
    {
        private MinerMonitorRoot.MinerTweakOperationResultsViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweakOperationResultsVm;
            }
        }

        public MinerTweakOperationResults()
        {
            this.DataContext = MinerMonitorRoot.MinerTweakOperationResultsVm;
            InitializeComponent();
        }

    }
}
