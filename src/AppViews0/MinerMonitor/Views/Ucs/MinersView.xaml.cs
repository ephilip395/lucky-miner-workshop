using Lucky.MinerMonitor.Vms;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinersView : UserControl
    {
        public MinersWindowViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweaksWindowVm;
            }
        }

        public MinersView()
        {
            this.DataContext = Vm;
            InitializeComponent();
        }

        private void MinerTweaksGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vm.SelectedMinerTweaks = ((DataGrid)sender).SelectedItems.Cast<MinerViewModel>().ToArray();
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WpfUtil.DataGrid_MouseDoubleClick<MinerViewModel>(sender, e, rowVm =>
            {
                rowVm.RemoteDesktop.Execute(null);
            });
        }
    }
}
