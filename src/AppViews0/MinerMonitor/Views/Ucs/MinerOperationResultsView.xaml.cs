using Lucky.MinerMonitor;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerOperationResultsView : UserControl
    {
        private MinerMonitorRoot.MinerTweakOperationResultsViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweakOperationResultsVm;
            }
        }

        public MinerOperationResultsView()
        {
            this.DataContext = MinerMonitorRoot.MinerTweakOperationResultsVm;
            InitializeComponent();
        }

    }
}
