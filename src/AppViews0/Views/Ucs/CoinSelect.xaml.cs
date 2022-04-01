﻿using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs
{
    public partial class CoinSelect : UserControl, IVmFrameworkElementTpl<CoinSelectViewModel>
    {
        public CoinSelectViewModel Vm { get; set; }

        public CoinSelect(CoinSelectViewModel vm)
        {
            this.Init(vm);
            InitializeComponent();
            this.OnLoaded(window =>
            {
                window.BuildEventPath<CoinAddedEvent>("添加了币种后，刷新币种选择下拉列表的Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message =>
                {
                    vm.OnPropertyChanged(nameof(vm.QueryResults));
                    vm.OnPropertyChanged(nameof(vm.HotCoins));
                });
                window.BuildEventPath<CoinRemovedEvent>("删除了币种后，刷新币种选择下拉列表的Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message =>
                {
                    vm.OnPropertyChanged(nameof(vm.QueryResults));
                    vm.OnPropertyChanged(nameof(vm.HotCoins));
                });
            });
        }

        private void KbButtonManageCoins_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Vm.HideView?.Execute(null);
        }

        private void DataGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Vm.OnOk?.Invoke(Vm.SelectedResult);
        }

        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Vm.OnOk?.Invoke(Vm.SelectedResult);
                e.Handled = true;
            }
        }
    }
}
