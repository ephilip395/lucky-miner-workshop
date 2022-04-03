using Lucky.Vms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lucky.Views.Ucs
{
    public partial class MinerProfileIndex : UserControl
    {
        public MinerProfileIndexViewModel Vm { get; private set; }

        public MinerProfileIndex()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
#if DEBUG
            NTStopwatch.Start();
#endif
            this.Vm = new MinerProfileIndexViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window =>
            {
                window.BuildEventPath<LocalContextReInitedEventHandledEvent>("上下文视图模型集刷新后刷新界面上的popup", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        UIThread.Execute(() =>
                        {
                            if (Vm.MinerProfile.MineWork != null)
                            {
                                return;
                            }
                            if (this.KernelFlyout.Content != null && this.KernelFlyout.IsOpen)
                            {
                                OpenKernelPopup();
                            }
                            if (this.MainCoinPoolFlyout.Content != null && this.MainCoinPoolFlyout.IsOpen)
                            {
                                OpenMainCoinPoolPopup();
                            }
                            if (this.MainCoinPool1Flyout.Content != null && this.MainCoinPool1Flyout.IsOpen)
                            {
                                OpenMainCoinPool1Popup();
                            }
                            if (this.MainCoinFlyout != null && this.MainCoinFlyout.IsOpen)
                            {
                                OpenMainCoinPopup();
                            }
                            if (this.MainCoinWalletFlyout != null && this.MainCoinPoolFlyout.IsOpen)
                            {
                                OpenMainCoinWalletPopup();
                            }
                        });
                    });
            });
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds)
            {
                LuckyConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
            }
#endif
        }

        private void DualCoinWeightSlider_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.CoinVm == null
                || Vm.MinerProfile.CoinVm.CoinKernel == null
                || Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile == null)
            {
                return;
            }
            CoinKernelProfileViewModel coinKernelProfileVm = Vm.MinerProfile.CoinVm.CoinKernel.CoinKernelProfile;
            LuckyContext.Instance.MinerProfile.SetCoinKernelProfileProperty(coinKernelProfileVm.CoinKernelId, nameof(coinKernelProfileVm.DualCoinWeight), coinKernelProfileVm.DualCoinWeight);
            LuckyContext.RefreshArgsAssembly.Invoke("主界面上的双挖权重拖动条失去焦点时");
        }

        #region OpenPopup

        private void OpenKernelPopup()
        {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null)
            {
                return;
            }
            var popup = KernelFlyout;
            var selected = coinVm.CoinKernel;
            CoinKernelSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Content == null || ((CoinKernelSelectViewModel)((CoinKernelSelect)popup.Content).DataContext).Coin != coinVm;
            if (newVm)
            {
                vm = new CoinKernelSelectViewModel(coinVm, selected, onOk: selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (coinVm.CoinKernel != selectedResult)
                        {
                            coinVm.CoinKernel = selectedResult;
                        }
                        else
                        {
                            selectedResult?.Kernel?.OnPropertyChanged(nameof(selectedResult.Kernel.FullName));
                        }
                        popup.Hide();
                    }
                })
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                };
            }
            if (popup.Content == null)
            {
                popup.Content = new CoinKernelSelect(vm);
            }
            else if (newVm)
            {
                ((CoinKernelSelect)popup.Content).DataContext = vm;
            }
            else
            {
                ((CoinKernelSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPoolPopup()
        {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null)
            {
                return;
            }
            var popup = MainCoinPoolFlyout;
            var selected = coinVm.CoinProfile.MainCoinPool;
            PoolSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Content == null || ((PoolSelectViewModel)((PoolSelect)popup.Content).DataContext).Coin != coinVm;
            if (newVm)
            {
                vm = new PoolSelectViewModel(coinVm, selected, onOk: selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (coinVm.CoinProfile.MainCoinPool != selectedResult)
                        {
                            coinVm.CoinProfile.MainCoinPool = selectedResult;
                        }
                        else
                        {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                        }
                        popup.Hide();
                    }
                })
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                };
            }
            if (popup.Content == null)
            {
                popup.Content = new PoolSelect(vm);
            }
            else if (newVm)
            {
                ((PoolSelect)popup.Content).DataContext = vm;
            }
            else
            {
                ((PoolSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPool1Popup()
        {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null)
            {
                return;
            }
            var popup = MainCoinPool1Flyout;
            var selected = coinVm.CoinProfile.MainCoinPool1;
            PoolSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Content == null || ((PoolSelectViewModel)((PoolSelect)popup.Content).DataContext).Coin != coinVm;
            if (newVm)
            {
                vm = new PoolSelectViewModel(coinVm, selected, onOk: selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (coinVm.CoinProfile.MainCoinPool1 != selectedResult)
                        {
                            coinVm.CoinProfile.MainCoinPool1 = selectedResult;
                        }
                        else
                        {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                        }
                        popup.Hide();
                    }
                }, usedByPool1: true)
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                };
            }
            if (popup.Content == null)
            {
                popup.Content = new PoolSelect(vm);
            }
            else if (newVm)
            {
                ((PoolSelect)popup.Content).DataContext = vm;
            }
            else
            {
                ((PoolSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinPopup()
        {
            var popup = MainCoinFlyout;
            var selected = Vm.MinerProfile.CoinVm;
            CoinSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Content == null || ((CoinSelectViewModel)((CoinSelect)popup.Content).DataContext).SelectedResult != selected;
            if (newVm)
            {
                vm = new CoinSelectViewModel(AppRoot.CoinVms.MainCoins.Where(a => a.IsSupported), selected, onOk: selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (Vm.MinerProfile.CoinVm != selectedResult)
                        {
                            Vm.MinerProfile.CoinVm = selectedResult;
                        }
                        else
                        {
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Code));
                        }
                        popup.Hide();
                    }
                }, isPromoteHotCoin: true)
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                };
            }
            if (popup.Content == null)
            {
                popup.Content = new CoinSelect(vm);
            }
            else if (newVm)
            {
                ((CoinSelect)popup.Content).DataContext = vm;
            }
            else
            {
                ((CoinSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }

        private void OpenMainCoinWalletPopup()
        {
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null || coinVm.CoinProfile == null)
            {
                return;
            }
            var popup = MainCoinWalletFlyout;
            var selected = coinVm.CoinProfile.SelectedWallet;
            bool isDualCoin = false;
            WalletSelectViewModel vm = null;
            // 如果服务器上下文刷新了则视图模型一定不等，因为上下文刷新后服务器视图模型会清空重建
            bool newVm = popup.Content == null || ((WalletSelectViewModel)((WalletSelect)popup.Content).DataContext).Coin != coinVm;
            if (newVm)
            {
                vm = new WalletSelectViewModel(coinVm, isDualCoin, selected, onOk: selectedResult =>
                {
                    if (selectedResult != null)
                    {
                        if (coinVm.CoinProfile.SelectedWallet != selectedResult)
                        {
                            coinVm.CoinProfile.SelectedWallet = selectedResult;
                        }
                        else
                        {
                            coinVm.CoinProfile.OnPropertyChanged(nameof(coinVm.CoinProfile.SelectedWallet));
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Name));
                            selectedResult.OnPropertyChanged(nameof(selectedResult.Address));
                        }
                        popup.Hide();
                    }
                })
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                };
            }
            if (popup.Content == null)
            {
                popup.Content = new WalletSelect(vm);
            }
            else if (newVm)
            {
                ((WalletSelect)popup.Content).DataContext = vm;
            }
        }

        #endregion

        private void KbButtonKernel_Clicked(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.IsMining)
            {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenKernelPopup();
            UserActionHappend();
        }

        private void KbButtonMainCoinPool_Clicked(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.IsMining)
            {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPoolPopup();
            UserActionHappend();
        }

        private void KbButtonMainCoin_Clicked(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.IsMining)
            {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPopup();
            UserActionHappend();
        }

        private void KbButtonMainCoinWallet_Clicked(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.IsMining)
            {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            var coinVm = Vm.MinerProfile.CoinVm;
            if (coinVm == null)
            {
                return;
            }
            if (coinVm.Wallets.Count == 0)
            {
                coinVm.CoinProfile?.AddWallet.Execute(null);
            }
            else
            {
                OpenMainCoinWalletPopup();
            }
        }

        private static void UserActionHappend()
        {
            if (!DevMode.IsDevMode)
            {
                VirtualRoot.RaiseEvent(new UserActionEvent());
            }
        }

        private void DualContainer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DualContainer.Visibility == Visibility.Visible && DualContainer.Child == null)
            {
                MinerProfileDual child = new MinerProfileDual();
                DualContainer.Child = child;
            }
        }

        private void KbButtonMainCoinPool1_Clicked(object sender, RoutedEventArgs e)
        {
            if (Vm.MinerProfile.IsMining)
            {
                VirtualRoot.Out.ShowWarn("请先停止挖矿", header: "提示", autoHideSeconds: 3);
                return;
            }
            OpenMainCoinPool1Popup();
            UserActionHappend();
        }

        private void BtnPopup_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VirtualRoot.Execute(new TopmostCommand());
        }

        private void BtnPopup_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VirtualRoot.Execute(new UnTopmostCommand());
        }
    }
}
