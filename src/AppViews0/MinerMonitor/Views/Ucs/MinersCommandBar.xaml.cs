using Lucky.MinerMonitor.Vms;
using Lucky.Views;
using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;


namespace Lucky.MinerMonitor.Views.Ucs
{
    /// <summary>
    /// Interaction logic for MinerTweaksCommandBar.xaml
    /// </summary>
    public partial class MinersCommandBar : UserControl
    {
        public MinersWindowViewModel Vm => MinerMonitorRoot.MinerTweaksWindowVm;

        public MinersCommandBar()
        {
            DataContext = Vm;
            InitializeComponent();
        }

        private void ItemMineWork_Click(object sender, RoutedEventArgs e)
        {
            var popup = MineWorkFlyout;
            MineWorkViewModel selected = null;
            if (Vm.SelectedMinerTweaks.Length == 1)
            {
                selected = Vm.SelectedMinerTweaks[0].SelectedMineWork;
            }
            if (popup.Content == null)
            {
                popup.Content = new MineWorkSelect(
                    new MineWorkSelectViewModel(
                        "为选中的矿机分配作业",
                        selected,
                        selectedResult =>
                        {
                            foreach (var item in Vm.SelectedMinerTweaks)
                            {
                                item.SelectedMineWork = selectedResult;
                            }
                            popup.Hide();
                        }
                    )
                    {
                        HideView = new DelegateCommand(() =>
                        {
                            popup.Hide();
                        })
                    }
                );
            }
            else
            {
                ((MineWorkSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }
        private void ItemMinerGroup_Click(object sender, RoutedEventArgs e)
        {
            var popup = MinerGroupFlyout;
            MinerGroupViewModel selected = null;
            if (Vm.SelectedMinerTweaks.Length == 1)
            {
                selected = Vm.SelectedMinerTweaks[0].SelectedMinerGroup;
            }
            if (popup.Content == null)
            {
                popup.Content = new MinerGroupSelect(new MinerGroupSelectViewModel("将选中的矿机放进分组", selected, selectedResult =>
                {
                    foreach (var item in Vm.SelectedMinerTweaks)
                    {
                        item.SelectedMinerGroup = selectedResult;
                    }
                    popup.Hide();
                })
                {
                    HideView = new DelegateCommand(() =>
                    {
                        popup.Hide();
                    })
                });
            }
            else
            {
                ((MinerGroupSelect)popup.Content).Vm.SelectedResult = selected;
            }
        }

        private void ItemUpgrade_Click(object sender, RoutedEventArgs e)
        {
            var popup = UpgradeFlyout;
            if (popup.Content == null)
            {
                popup.Content = new LuckyFileSelect(
                    new LuckyFileSelectViewModel(selectedResult =>
                    {
                        if (selectedResult == null || selectedResult == LuckyFileViewModel.Empty)
                        {
                            return;
                        }
                        DialogWindow.ShowSoftDialog(
                            new DialogWindowViewModel(
                                message: selectedResult.Description,
                                title: $"确定将选中的矿机升级到{selectedResult.Version}吗？",
                                onYes: () =>
                                {
                                    foreach (var item in Vm.SelectedMinerTweaks)
                                    {
                                        MinerMonitorRoot.MinerMonitorService.UpgradeLuckyAsync(item, selectedResult.FileName);
                                    }
                                }
                            )
                        );
                        popup.Hide();
                    })
                    {
                        HideView = new DelegateCommand(() =>
                        {
                            popup.Hide();
                        })
                    });
            }
            else
            {
                ((LuckyFileSelect)popup.Content).Vm.SelectedResult = null;
                VirtualRoot.Execute(new RefreshLuckyFileSetCommand());
            }
        }

        private void OpenMessagesWindow(object sender, RoutedEventArgs e)
        {
            var window = new MessagesWindow
            {
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }

        private void OpenOutputWindow(object sender, RoutedEventArgs e)
        {
            var window = new OutputWindow
            {
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }

        private void OpenMinerDetailsWindow(object sender, RoutedEventArgs e)
        {
            var window = new MinerDetailsWindow
            {
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }
    }
}
