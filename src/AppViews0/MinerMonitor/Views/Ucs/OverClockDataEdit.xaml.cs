using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.MinerMonitor.Views.Ucs {
    public partial class OverClockDataEdit : UserControl {
        public static void ShowWindow(FormType formType, OverClockDataViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "超频菜谱",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 250,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_OverClock"
            }, ucFactory: (window) => {
                OverClockDataViewModel vm = new OverClockDataViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new OverClockDataEdit(vm);
            }, fixedSize: true);
        }

        public OverClockDataViewModel Vm { get; private set; }

        public OverClockDataEdit(OverClockDataViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
