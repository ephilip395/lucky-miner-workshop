using Lucky.MinerMonitor.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace Lucky.MinerMonitor.Views.Ucs
{
    /// <summary>
    /// Interaction logic for MinerTweaksFilterBar.xaml
    /// </summary>
    public partial class MinersFilterBar : UserControl
    {
        public MinersWindowViewModel Vm => MinerMonitorRoot.MinerTweaksWindowVm;

        public MinersFilterBar()
        {
            InitializeComponent();
            DataContext = Vm;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

    }
}
