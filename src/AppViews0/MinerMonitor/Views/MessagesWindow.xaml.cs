using Lucky.MinerMonitor.Vms;

using System;
using System.Windows;

namespace Lucky.MinerMonitor.Views
{
    /// <summary>
    /// Interaction logic for MessagesWindow.xaml
    /// </summary>
    public partial class MessagesWindow : Window
    {
        public MinerTweaksWindowViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweaksWindowVm;
            }
        }
        public MessagesWindow()
        {
            DataContext = Vm;
            MinerMonitorRoot.SetIsMinerTweakMessagesVisible(true);
            InitializeComponent();
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MinerMonitorRoot.SetIsMinerTweakMessagesVisible(false);

        }
    }
}
