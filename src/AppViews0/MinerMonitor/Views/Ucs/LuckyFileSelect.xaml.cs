using Lucky.MinerMonitor.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class LuckyFileSelect : UserControl
    {
        public LuckyFileSelectViewModel Vm { get; private set; }

        public LuckyFileSelect(LuckyFileSelectViewModel vm)
        {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vm.OnOk?.Invoke(Vm.SelectedResult);
        }

        private void HideView(object sender, System.Windows.RoutedEventArgs e)
        {
            Vm.HideView?.Execute(null);
        }
    }
}
