using Lucky.MinerMonitor.Vms;
using System.Windows;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerTweakAdd : Window
    {
        public static void ShowWindow()
        {
            MinerTweakAddViewModel vm = new MinerTweakAddViewModel();
            Window window = new MinerTweakAdd(vm);
            window.BuildCloseWindowOncePath(vm.Id);
            //window.MousePosition();
            window.ShowSoftDialog();
        }

        public MinerTweakAdd(MinerTweakAddViewModel vm)
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
