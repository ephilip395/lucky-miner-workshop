﻿using Lucky.Vms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Lucky.Views.Ucs
{
    /// <summary>
    /// Interaction logic for ProfitView.xaml
    /// </summary>
    public partial class ProfitView : UserControl
    {
        public StateBarViewModel Vm { get; private set; }

        public ProfitView()
        {
            this.Vm = new StateBarViewModel();
            this.DataContext = Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            this.OnLoaded(window =>
            {
                window.Activated += (object sender, EventArgs e) =>
                {
                    Vm.OnPropertyChanged(nameof(Vm.IsAutoAdminLogon));
                    Vm.OnPropertyChanged(nameof(Vm.AutoAdminLogonToolTip));
                    VirtualRoot.Execute(new RefreshIsRemoteDesktopEnabledCommand());
                };
                window.BuildEventPath<PoolDelayPickedEvent>("从内核输出中提取了矿池延实时展示到界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        if (message.IsDual)
                        {
                            Vm.DualPoolDelayText = message.PoolDelayText;
                        }
                        else
                        {
                            Vm.PoolDelayText = message.PoolDelayText;
                        }
                    });
                window.BuildEventPath<MineStartedEvent>("开始挖矿后清空矿池延时 & 挖矿开始后将内核自我重启计数清零", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.PoolDelayText = string.Empty;
                        Vm.DualPoolDelayText = string.Empty;
                        Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                    });
                window.BuildEventPath<MineStopedEvent>("停止挖矿后将清空矿池延时", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.PoolDelayText = string.Empty;
                        Vm.DualPoolDelayText = string.Empty;
                    });
                window.BuildEventPath<CpuPackageStateChangedEvent>("CPU包状态变更后刷新Vm内存", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        UpdateCpuView();
                    });
                window.BuildEventPath<LocalIpSetInitedEvent>("本机IP集刷新后刷新状态栏", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.RefreshLocalIps();
                    });
                window.BuildEventPath<MinutePartChangedEvent>("时间的分钟部分变更过更新计时器显示", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.UpdateDateTime();
                    });
                window.BuildEventPath<Per1SecondEvent>("挖矿计时秒表", LogEnum.None, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        DateTime now = DateTime.Now;
                        Vm.UpdateBootTimeSpan(now - LuckyContext.Instance.CreatedOn);
                        var mineContext = LuckyContext.Instance.LockedMineContext;
                        if (mineContext != null && mineContext.MineStartedOn != DateTime.MinValue)
                        {
                            Vm.UpdateMineTimeSpan(now - mineContext.MineStartedOn);
                        }
                    });
                window.BuildEventPath<AppVersionChangedEvent>("发现了服务端新版本", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.SetCheckUpdateForeground(isLatest: EntryAssemblyInfo.CurrentVersion >= LuckyContext.ServerVersion);
                    });
                window.BuildEventPath<KernelSelfRestartedEvent>("内核自我重启时刷新计数器", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message =>
                    {
                        Vm.OnPropertyChanged(nameof(Vm.KernelSelfRestartCountText));
                    });
                window.BuildCmdPath<RefreshIsRemoteDesktopEnabledCommand>(LogEnum.DevConsole,
                    path: message =>
                    {
                        Vm.RefreshIsRemoteDesktopEnabled();
                    }, location: this.GetType());
            });
            var gpuSet = LuckyContext.Instance.GpuSet;
            // 建议每张显卡至少对应4G虚拟内存，否则标红
            if (VirtualRoot.DriveSet.OSVirtualMemoryMb < gpuSet.Count * AppRoot.OsVmPerGpu * LuckyKeyword.IntK)
            {
                BtnShowVirtualMemory.Foreground = WpfUtil.RedBrush;
                BtnShowVirtualMemory.ToolTip = "点击打开虚拟内存设置页，如果磁盘充足建议调高虚拟内存";
            }
            else
            {
                BtnShowVirtualMemory.ToolTip = "虚拟内存";
            }
        }

        #region 更新状态栏展示的CPU使用率和温度
        private int _cpuPerformance = 0;
        private int _cpuTemperature = 0;
        private void UpdateCpuView()
        {
            int performance = LuckyContext.Instance.CpuPackage.Performance;
            int temperature = LuckyContext.Instance.CpuPackage.Temperature;
            if (temperature < 0)
            {
                temperature = 0;
            }
            if (_cpuPerformance != performance)
            {
                _cpuPerformance = performance;
                Vm.CpuPerformanceText = performance.ToString() + "%";
            }
            if (_cpuTemperature != temperature)
            {
                _cpuTemperature = temperature;
                Vm.CpuTemperatureText = temperature.ToString() + " ℃";
            }
        }
        #endregion

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (this.UpdateIcon.Visibility == Visibility.Visible)
            {
                Process process = Process.GetProcessesByName(LuckyKeyword.LuckyUpdaterProcessName).FirstOrDefault();
                if (process == null)
                {
                    this.UpdateIcon.Visibility = Visibility.Collapsed;
                    this.LoadingIcon.Visibility = Visibility.Visible;
                    // 这里的逻辑是每100毫秒检查一次升级器进程是否存在，每检查一次将loading图标
                    // 旋转30度，如果升级器进程存在了或者已经检查了3秒钟了则停止检查。
                    VirtualRoot.SetInterval(
                        per: TimeSpan.FromMilliseconds(100),
                        perCallback: () =>
                        {
                            UIThread.Execute(() =>
                            {
                                ((RotateTransform)this.LoadingIcon.RenderTransform).Angle += 30;
                            });
                        },
                        stopCallback: () =>
                        {
                            UIThread.Execute(() =>
                            {
                                this.UpdateIcon.Visibility = Visibility.Visible;
                                this.LoadingIcon.Visibility = Visibility.Collapsed;
                                ((RotateTransform)this.LoadingIcon.RenderTransform).Angle = 0;
                            });
                        },
                        timeout: TimeSpan.FromSeconds(3),
                        requestStop: () =>
                        {
                            return Process.GetProcessesByName(LuckyKeyword.LuckyUpdaterProcessName).FirstOrDefault() != null;
                        }
                    );
                }
            }
        }

        private void PanelEprice_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.ShowInputDialog("电价", Vm.MinerProfile.EPrice.ToString("f2"), "￥ / 度", eprice =>
            {
                if (!double.TryParse(eprice, out _))
                {
                    return "电价必须是数字";
                }
                return string.Empty;
            }, onOk: eprice =>
            {
                Vm.MinerProfile.EPrice = double.Parse(eprice);
            });
        }
    }
}
