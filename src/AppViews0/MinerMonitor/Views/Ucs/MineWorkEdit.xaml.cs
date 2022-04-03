using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows.Controls;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MineWorkEdit : UserControl
    {
        public static void ShowWindow(FormType formType, MineWorkViewModel source)
        {
            string title = source.IsSelfMineWork ? "单机作业" : "挖矿作业 — 作业通常用于让不同的矿机执行同样的挖矿任务";
            ContainerWindow.ShowWindow(new ContainerWindowViewModel
            {
                Title = title,
                FormType = formType,
                IsMaskTheParent = true,
                Width = 1000,
                Height = 560,
                CloseVisible = Visibility.Visible,
                IconName = "Icon_MineWork"
            }, ucFactory: (window) =>
            {
                MineWorkViewModel vm = new MineWorkViewModel(source);
                window.Closed += (sender, e) =>
                {
                    vm.Save.Execute(null);
                };
                //NotiCenterWindow.Bind(window);
                return new MineWorkEdit(vm);
            }, beforeShow: (window, uc) =>
            {
                LuckyContext.RefreshArgsAssembly.Invoke("打开编辑挖矿作业页面时");
            }, fixedSize: false);
        }

        public MineWorkViewModel Vm { get; private set; }

        public MineWorkEdit(MineWorkViewModel vm)
        {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
