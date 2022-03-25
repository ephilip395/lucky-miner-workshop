using Lucky.MinerMonitor.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Views.Ucs
{
    public partial class MinerTweaksPagging : UserControl
    {
        public MinerTweaksWindowViewModel Vm
        {
            get
            {
                return MinerMonitorRoot.MinerTweaksWindowVm;
            }
        }

        public MinerTweaksPagging()
        {
            InitializeComponent();
        }

        private void KbButtonColumnsShow_Clicked(object sender, RoutedEventArgs e)
        {
            var popup = PopupColumnsShow;
            var selected = Vm.ColumnsShow;
            if (popup.Child == null)
            {
                popup.Child = new ColumnsShowSelect(new ColumnsShowSelectViewModel(selected, selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (Vm.ColumnsShow != selectedResult)
                        {
                            Vm.ColumnsShow = selectedResult;
                        }
                        popup.IsOpen = false;
                    }
                })
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.IsOpen = false;
                    })
                });
            }
            else
            {
                ((ColumnsShowSelect)popup.Child).Vm.SelectedResult = selected;
            }
            popup.IsOpen = true;
        }

        public void TextBoxPageIndex_KeyUp(object sender, KeyEventArgs e)
        {
            WpfUtil.TextBoxPageIndex_KeyUp(sender, e);
        }
    }
}
