using Lucky.Vms;
using System;
using System.Windows.Controls;

namespace Lucky.Views.Ucs
{
    public partial class CoinPage : UserControl
    {
        public static void ShowWindow(CoinViewModel currentCoin, string tabType)
        {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel
            {
                Title = "币种",
                IconName = "Icon_Coin",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = DevMode.IsDevMode ? AppRoot.MainWindowWidth : 1000,
                Height = 520
            },
            ucFactory: (window) => new CoinPage(),
            beforeShow: (window, uc) =>
            {
                if (currentCoin != null)
                {
                    switch (tabType)
                    {
                        case LuckyKeyword.PoolParameterName:
                            uc.Vm.IsPoolTabSelected = true;
                            break;
                        case LuckyKeyword.WalletParameterName:
                            uc.Vm.IsWalletTabSelected = true;
                            break;
                        default:
                            break;
                    }
                    uc.Vm.CurrentCoin = currentCoin;
                }
            });
        }

        public CoinPageViewModel Vm { get; private set; }

        public CoinPage()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            this.Vm = new CoinPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            AppRoot.CoinVms.PropertyChanged += Current_PropertyChanged;
            this.Unloaded += CoinPage_Unloaded;
        }

        private void CoinPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AppRoot.CoinVms.PropertyChanged -= Current_PropertyChanged;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppRoot.CoinVms.AllCoins))
            {
                Vm.OnPropertyChanged(nameof(Vm.QueryResults));
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.DataGrid_EditRow<CoinViewModel>(sender, e);
        }

        private void WalletDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.DataGrid_EditRow<WalletViewModel>(sender, e);
        }

        private void PoolDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.DataGrid_EditRow<PoolViewModel>(sender, e);
        }

        private void KernelDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.DataGrid_EditRow<CoinKernelViewModel>(sender, e);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
