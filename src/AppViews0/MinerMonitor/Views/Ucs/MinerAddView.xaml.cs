using Lucky.MinerMonitor.Vms;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerAddView : Window
    {
        public static void ShowWindow()
        {
            MinerAddViewModel vm = new MinerAddViewModel();
            Window window = new MinerAddView(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            //window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerAddView(MinerAddViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
            var owner = WpfUtil.GetTopWindow();
            if (this != owner)
            {
                this.Owner = owner;
            }
        }
    }
}
