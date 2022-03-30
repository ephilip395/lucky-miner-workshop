using Lucky.MinerMonitor;
using Lucky.View;
using Lucky.Views;
using Lucky.Views.Ucs;
using Lucky.Vms;
using System.Windows;
using MinerTweakUcs = Lucky.Views.Ucs;
using MinerMonitorUcs = Lucky.MinerMonitor.Views.Ucs;
using MinerMonitorViews = Lucky.MinerMonitor.Views;

namespace Lucky
{
    public class AppViewFactory : AbstractAppViewFactory
    {
        public AppViewFactory() { }

        public override Window CreateMainWindow()
        {
            return new MainWindow();
        }

        public override void BuildPaths()
        {

            var location = this.GetType();

            // 处理显示对话框命令
            VirtualRoot.BuildCmdPath<ShowDialogWindowCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(message: message.Message, title: message.Title, onYes: message.OnYes, icon: message.Icon));
                });
            });

            // 显示盈利计算器
            VirtualRoot.BuildCmdPath<ShowCalcCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    Calc.ShowWindow(message.CoinVm);
                });
            });
            // 显示本地IP
            VirtualRoot.BuildCmdPath<ShowLocalIpsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    LocalIpConfig.ShowWindow();
                });
            });
            // 显示关于页面
            VirtualRoot.BuildCmdPath<ShowAboutPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    AboutPage.ShowWindow();
                });
            });
            // 显示内核输出页面
            VirtualRoot.BuildCmdPath<ShowKernelOutputPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelOutputPage.ShowWindow(message.SelectedKernelOutputVm);
                });
            });
            // 显示内核输入页面
            VirtualRoot.BuildCmdPath<ShowKernelInputPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelInputPage.ShowWindow();
                });
            });
            // 显示标签品牌
            VirtualRoot.BuildCmdPath<ShowTagBrandCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                if (LuckyContext.IsBrandSpecified)
                {
                    return;
                }
                UIThread.Execute(() =>
                {
                    BrandTag.ShowWindow();
                });
            });
            // 显示加密货币页面
            VirtualRoot.BuildCmdPath<ShowCoinPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    CoinPage.ShowWindow(message.CurrentCoin, message.TabType);
                });
            });
            // 显示加密货币分组
            VirtualRoot.BuildCmdPath<ShowCoinGroupsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    CoinGroupPage.ShowWindow();
                });
            });

            // 显示系统词典页面
            VirtualRoot.BuildCmdPath<ShowSysDicPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    SysDicPage.ShowWindow();
                });
            });

            // 显示虚拟内存
            VirtualRoot.BuildCmdPath<ShowVirtualMemoryCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerTweakUcs.VirtualMemory.ShowWindow();
                });
            });

            // 显示重启系统
            VirtualRoot.BuildCmdPath<ShowRestartWindowsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    RestartWindows.ShowDialog(new RestartWindowsViewModel(message.CountDownSeconds));
                });
            });
            // 显示通知
            VirtualRoot.BuildCmdPath<ShowNotificationSampleCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    NotificationSample.ShowWindow();
                });
            });

            // 显示属性
            VirtualRoot.BuildCmdPath<ShowPropertyCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    Property.ShowWindow();
                });
            });

            // 显示消息路径Ids
            VirtualRoot.BuildCmdPath<ShowMessagePathIdsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MessagePathIds.ShowWindow();
                });
            });

            // 显示内核窗口
            VirtualRoot.BuildCmdPath<ShowKernelsWindowCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelsWindow.ShowWindow();
                });
            });

            // 显示内核下载器
            VirtualRoot.BuildCmdPath<ShowKernelDownloaderCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelDownloading.ShowWindow(message.KernelId, message.DownloadComplete);
                });
            });
            // 编辑环境变量
            VirtualRoot.BuildCmdPath<EditEnvironmentVariableCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                });
            });
            // 编辑输入段
            VirtualRoot.BuildCmdPath<EditInputSegmentCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                });
            });
            // 编辑货币内核
            VirtualRoot.BuildCmdPath<EditCoinKernelCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑货币
            VirtualRoot.BuildCmdPath<EditCoinCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    CoinEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 显示速度图
            VirtualRoot.BuildCmdPath<ShowSpeedChartsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    SpeedCharts.ShowWindow(message.GpuSpeedVm);
                });
            });
            // 显示文件 writer
            VirtualRoot.BuildCmdPath<ShowFileWriterPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    FileWriterPage.ShowWindow();
                });
            });
            // 编辑文件 writer
            VirtualRoot.BuildCmdPath<EditFileWriterCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    FileWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 显示 fragment writer 页面
            VirtualRoot.BuildCmdPath<ShowFragmentWriterPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    FragmentWriterPage.ShowWindow();
                });
            });
            // 编辑 fragment writer
            VirtualRoot.BuildCmdPath<EditFragmentWriterCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    FragmentWriterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑分组
            VirtualRoot.BuildCmdPath<EditGroupCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    GroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑服务器消息
            VirtualRoot.BuildCmdPath<EditServerMessageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    ServerMessageEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑内核输入
            VirtualRoot.BuildCmdPath<EditKernelInputCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelInputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑内核输出关键字
            VirtualRoot.BuildCmdPath<EditKernelOutputKeywordCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelOutputKeywordEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑内核输入翻译
            VirtualRoot.BuildCmdPath<EditKernelOutputTranslaterCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 编辑内核输出
            VirtualRoot.BuildCmdPath<EditKernelOutputCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                });
            });
            // 显示包窗口
            VirtualRoot.BuildCmdPath<ShowPackagesWindowCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    PackagesWindow.ShowWindow();
                });
            });

            // 编辑内核
            VirtualRoot.BuildCmdPath<EditKernelCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑包
            VirtualRoot.BuildCmdPath<EditPackageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    PackageEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑矿池内核
            VirtualRoot.BuildCmdPath<EditPoolKernelCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑矿池
            VirtualRoot.BuildCmdPath<EditPoolCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    PoolEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑系统词典条目
            VirtualRoot.BuildCmdPath<EditSysDicItemCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑系统词典
            VirtualRoot.BuildCmdPath<EditSysDicCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    SysDicEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 显示内核输出关键字
            VirtualRoot.BuildCmdPath<ShowKernelOutputKeywordsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    KernelOutputKeywords.ShowWindow();
                });
            });

            // 编辑钱包
            VirtualRoot.BuildCmdPath<EditWalletCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    WalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 矿工监控端

            #region MinerMonitor

            // 显示计算器配置
            VirtualRoot.BuildCmdPath<ShowCalcConfigCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.CalcConfig.ShowWindow();
                });
            });

            // 显示矿工客户端窗口
            VirtualRoot.BuildCmdPath<ShowMinerTweaksWindowCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorViews.MinersWindow.ShowWindow(message.IsToggle);
                });
            });

            // 显示挖矿者升级配置
            VirtualRoot.BuildCmdPath<ShowLuckyUpdaterConfigCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.LuckyUpdaterConfig.ShowWindow();
                });
            });

            // 显示挖矿者寻找配置
            VirtualRoot.BuildCmdPath<ShowMinerFinderConfigCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MinerFinderConfig.ShowWindow();
                });
            });

            // 显示超频数据页面
            VirtualRoot.BuildCmdPath<ShowOverClockDataPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.OverClockDataPage.ShowWindow();
                });
            });


            // 显示监控端虚拟内存
            VirtualRoot.BuildCmdPath<ShowMinerMonitorVirtualMemoryCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.VirtualMemory.ShowWindow(message.Vm);
                });
            });

            // 显示监控端本地IP
            VirtualRoot.BuildCmdPath<ShowMinerMonitorLocalIpsCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.LocalIpConfig.ShowWindow(message.Vm);
                });
            });

            // 显示挖矿者钱包页面
            VirtualRoot.BuildCmdPath<ShowLuckyWalletPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.LuckyWalletPage.ShowWindow();
                });
            });

            // 显示用户页面
            VirtualRoot.BuildCmdPath<ShowUserPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.UserPage.ShowWindow();
                });
            });

            // 显示GPU名字页面
            VirtualRoot.BuildCmdPath<ShowGpuNamePageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.GpuNameCounts.ShowWindow();
                });
            });

            // 显示动作计数页面
            VirtualRoot.BuildCmdPath<ShowActionCountPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.ActionCounts.ShowWindow();
                });
            });

            // 显示MQ计数页面
            VirtualRoot.BuildCmdPath<ShowMqCountsPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MqCountsPage.ShowWindow();
                });
            });

            // 显示修改密码
            VirtualRoot.BuildCmdPath<ShowChangePassword>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.ChangePassword.ShowWindow();
                });
            });

            // 显示WS服务器节点页面
            VirtualRoot.BuildCmdPath<ShowWsServerNodePageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.WsServerNodePage.ShowWindow();
                });
            });

            // 显示远程桌面登录对话框
            VirtualRoot.BuildCmdPath<ShowRemoteDesktopLoginDialogCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.RemoteDesktopLogin.ShowWindow(message.Vm);
                });
            });

            // 显示矿工客户端设置
            VirtualRoot.BuildCmdPath<ShowMinerTweakSettingCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MinerSettingView.ShowWindow(message.Vm);
                });
            });

            // 显示矿工名字设置
            VirtualRoot.BuildCmdPath<ShowMinerNamesSeterCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MinerNamesSeter.ShowWindow(message.Vm);
                });
            });

            // 显示 GPU Profiles 页面
            VirtualRoot.BuildCmdPath<ShowGpuProfilesPageCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.GpuProfilesPage.ShowWindow(message.MinerTweaksWindowVm);
                });
            });

            // 显示矿工客户端增加
            VirtualRoot.BuildCmdPath<ShowMinerTweakAddCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MinerAddView.ShowWindow();
                });
            });

            // 编辑矿工分组
            VirtualRoot.BuildCmdPath<EditMinerGroupCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑挖矿者钱包
            VirtualRoot.BuildCmdPath<EditLuckyWalletCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.LuckyWalletEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑矿工工作
            VirtualRoot.BuildCmdPath<EditMineWorkCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.MineWorkEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑超频数据
            VirtualRoot.BuildCmdPath<EditOverClockDataCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                });
            });

            // 编辑要显示的列
            VirtualRoot.BuildCmdPath<EditColumnsShowCommand>(location: location, LogEnum.DevConsole, path: message =>
            {
                UIThread.Execute(() =>
                {
                    MinerMonitorUcs.ColumnsShowEdit.ShowWindow(message.Source);
                });
            });
            #endregion
        }
    }
}
