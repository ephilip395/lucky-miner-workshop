﻿using Lucky.Core;
using Lucky.FileOpeners;
using Lucky.FileOpeners.Impl;
using Lucky.Hub;
using Lucky.RemoteDesktop;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Lucky
{
    /// <summary>
    /// 该类型不是用于绑定到视图的，AppStatic才是
    /// </summary>
    public static partial class AppRoot
    {
        public static Action<RdpInput> RemoteDesktop;
        public static ExtendedNotifyIcon NotifyIcon;
        public const string LowWinMessage = "Windows版本较低，建议使用Win10系统";
        // everedit存在试用期，试用期过后无法继续使用，所以需要改回使用notepad++
        public static readonly IFileOpener FileOpener = new NppFileOpener();

        private static readonly List<IMessagePathId> _contextPathIds = new List<IMessagePathId>();

        static AppRoot()
        {
        }

        #region methods
        // 因为是上下文路径，无需返回路径标识
        public static void BuildCmdPath<TCmd>(LogEnum logType, Type location, Action<TCmd> path)
            where TCmd : ICmd
        {
            var messagePathId = VirtualRoot.BuildCmdPath(location, logType, path);
            _contextPathIds.Add(messagePathId);
        }

        // 因为是上下文路径，无需返回路径标识
        public static void BuildEventPath<TEvent>(string description, LogEnum logType, Type location, PathPriority priority, Action<TEvent> path)
            where TEvent : IEvent
        {
            var messagePathId = VirtualRoot.BuildMessagePath(description, logType, location, priority, path);
            _contextPathIds.Add(messagePathId);
        }

        /// <summary>
        /// 解封路
        /// </summary>
        public static void Enable()
        {
            foreach (var pathId in _contextPathIds)
            {
                pathId.IsEnabled = true;
            }
        }

        /// <summary>
        /// 封路，禁止通行。没多大意义。
        /// </summary>
        public static void Disable()
        {
            foreach (var pathId in _contextPathIds)
            {
                pathId.IsEnabled = false;
            }
        }
        #endregion

        #region MainWindowHeight MainWindowWidth
        public static double MainWindowHeight
        {
            get
            {
                if (SystemParameters.WorkArea.Size.Height >= 630)
                {
                    return 630;
                }
                else if (SystemParameters.WorkArea.Size.Height >= 520)
                {
                    return 520;
                }
                return 480;
            }
        }

        public static double MainWindowWidth
        {
            get
            {
                if (SystemParameters.WorkArea.Size.Width >= 1090)
                {
                    return 1090;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 1000)
                {
                    return 1000;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 860)
                {
                    return 860;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 800)
                {
                    return 800;
                }
                return 640;
            }
        }
        #endregion

        #region 字典项
        public static string NppPackageUrl
        {
            get
            {
                string url = $"https://{LuckyKeyword.CloudFileDomain}/tool/npp.zip";
                if (WpfUtil.IsInDesignMode)
                {
                    return url;
                }
                return LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue("Tool", "npp", defaultValue: url);
            }
        }

        public static string EvereditPackageUrl
        {
            get
            {
                string url = $"https://{LuckyKeyword.CloudFileDomain}/tool/everedit.zip";
                if (WpfUtil.IsInDesignMode)
                {
                    return url;
                }
                return LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue("Tool", "everedit", defaultValue: url);
            }
        }
        #endregion

        public static double OsVmPerGpu
        {
            get
            {
                double value = 5.0;
                if (WpfUtil.IsInDesignMode)
                {
                    return value;
                }
                return LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(LuckyKeyword.ThisSystemSysDicCode, LuckyKeyword.OsVmPerGpuSysDicItemCode, value);
            }
        }

        public static string VirtualMemoryDescription
        {
            get
            {
                return $"挖 ETH 每卡至少设置 {OsVmPerGpu.ToString("f1")}G 虚拟内存，建议按虚拟内存比显存 1 比 1 设置。比如 6 张 6G 的 1066 显卡建议设置 6 x 6 = 36G。";
            }
        }

        #region Upgrade
        private static string GetUpdaterVersion()
        {
            string version = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(LuckyKeyword.UpdaterVersionAppSettingKey, out IAppSetting setting) && setting.Value != null)
            {
                version = setting.Value.ToString();
            }
            return version;
        }

        private static void SetUpdaterVersion(string value)
        {
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
            {
                Key = LuckyKeyword.UpdaterVersionAppSettingKey,
                Value = value
            }));
        }

        public static void Upgrade(string fileName, Action callback)
        {
            RpcRoot.OfficialServer.FileUrlService.GetLuckyUpdaterUrlAsync((downloadFileUrl, e) =>
            {
                try
                {
                    string argument = string.Empty;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        argument = "luckycmFileName=" + fileName;
                    }
                    if (ClientAppType.IsMinerMonitor)
                    {
                        argument += " --minerstudio";
                    }
                    if (string.IsNullOrEmpty(downloadFileUrl))
                    {
                        if (File.Exists(HomePath.UpdaterFileFullName))
                        {
                            Windows.Cmd.RunClose(HomePath.UpdaterFileFullName, argument);
                        }
                        callback?.Invoke();
                        return;
                    }
                    Uri uri = new Uri(downloadFileUrl);
                    string localVersion = GetUpdaterVersion();
                    if (string.IsNullOrEmpty(localVersion) || !File.Exists(HomePath.UpdaterFileFullName) || uri.AbsolutePath != localVersion)
                    {
                        VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "行运矿工更新器", (window, isSuccess, message, saveFileFullName) =>
                        {
                            try
                            {
                                if (isSuccess)
                                {
                                    string updateDirFullName = Path.GetDirectoryName(HomePath.UpdaterFileFullName);
                                    if (!Directory.Exists(updateDirFullName))
                                    {
                                        Directory.CreateDirectory(updateDirFullName);
                                    }
                                    File.Delete(HomePath.UpdaterFileFullName);
                                    File.Move(saveFileFullName, HomePath.UpdaterFileFullName);
                                    SetUpdaterVersion(uri.AbsolutePath);
                                    window?.Close();
                                    Windows.Cmd.RunClose(HomePath.UpdaterFileFullName, argument);
                                    callback?.Invoke();
                                }
                                else
                                {
                                    VirtualRoot.MyLocalError(nameof(AppRoot), "下载新版本：" + message, toConsole: true);
                                    callback?.Invoke();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorDebugLine(ex);
                                callback?.Invoke();
                            }
                        }));
                    }
                    else
                    {
                        Windows.Cmd.RunClose(HomePath.UpdaterFileFullName, argument);
                        callback?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorDebugLine(ex);
                    callback?.Invoke();
                }
            });
        }
        #endregion

        #region OpenMinerFinder
        private static string GetMinerFinderVersion()
        {
            string version = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(LuckyKeyword.MinerFinderVersionAppSettingKey, out IAppSetting setting) && setting.Value != null)
            {
                version = setting.Value.ToString();
            }
            return version;
        }

        private static void SetMinerFinderVersion(string value)
        {
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
            {
                Key = LuckyKeyword.MinerFinderVersionAppSettingKey,
                Value = value
            }));
        }

        public static void OpenMinerFinder()
        {
            RpcRoot.OfficialServer.FileUrlService.GetMinerFinderUrlAsync((downloadFileUrl, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(downloadFileUrl))
                    {
                        if (File.Exists(MinerTweakTempPath.MinerFinderFileFullName))
                        {
                            Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                        }
                        return;
                    }
                    Uri uri = new Uri(downloadFileUrl);
                    string localVersion = GetMinerFinderVersion();
                    if (string.IsNullOrEmpty(localVersion) || !File.Exists(MinerTweakTempPath.MinerFinderFileFullName) || uri.AbsolutePath != localVersion)
                    {
                        VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "下载矿机雷达", (window, isSuccess, message, saveFileFullName) =>
                        {
                            try
                            {
                                if (isSuccess)
                                {
                                    File.Delete(MinerTweakTempPath.MinerFinderFileFullName);
                                    File.Move(saveFileFullName, MinerTweakTempPath.MinerFinderFileFullName);
                                    SetMinerFinderVersion(uri.AbsolutePath);
                                    window?.Close();
                                    Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                                }
                                else
                                {
                                    VirtualRoot.MyLocalError(nameof(AppRoot), "下载矿机雷达：" + message, toConsole: true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorDebugLine(ex);
                            }
                        }));
                    }
                    else
                    {
                        Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorDebugLine(ex);
                }
            });
        }
        #endregion

        public static void OpenMinerFinder1()
        {
            string downloadFileUrl = $"{LuckyKeyword.CloudToolsUrlBase}/{LuckyKeyword.MinerFinderFileName}";
            try
            {
                if (string.IsNullOrEmpty(downloadFileUrl))
                {
                    if (File.Exists(MinerTweakTempPath.MinerFinderFileFullName))
                    {
                        Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                    }
                    return;
                }
                Uri uri = new Uri(downloadFileUrl);
                string localVersion = GetMinerFinderVersion();
                if (string.IsNullOrEmpty(localVersion) || !File.Exists(MinerTweakTempPath.MinerFinderFileFullName) || uri.AbsolutePath != localVersion)
                {
                    VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "下载矿机雷达", (window, isSuccess, message, saveFileFullName) =>
                    {
                        try
                        {
                            if (isSuccess)
                            {
                                File.Delete(MinerTweakTempPath.MinerFinderFileFullName);
                                File.Move(saveFileFullName, MinerTweakTempPath.MinerFinderFileFullName);
                                SetMinerFinderVersion(uri.AbsolutePath);
                                window?.Close();
                                Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                            }
                            else
                            {
                                VirtualRoot.MyLocalError(nameof(AppRoot), "下载矿机雷达：" + message, toConsole: true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.ErrorDebugLine(ex);
                        }
                    }));
                }
                else
                {
                    Windows.Cmd.RunClose(MinerTweakTempPath.MinerFinderFileFullName, string.Empty, waitForExit: false);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorDebugLine(ex);
            }
        }

        #region OpenAtikmdagPatcher
        private static string GetAtikmdagPatcherVersion()
        {
            string version = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(LuckyKeyword.AtikmdagPatcherVersionAppSettingKey, out IAppSetting setting) && setting.Value != null)
            {
                version = setting.Value.ToString();
            }
            return version;
        }

        private static void SetAtikmdagPatcherVersion(string value)
        {
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
            {
                Key = LuckyKeyword.AtikmdagPatcherVersionAppSettingKey,
                Value = value
            }));
        }

        public static void OpenAtikmdagPatcher()
        {
            RpcRoot.OfficialServer.FileUrlService.GetAtikmdagPatcherUrlAsync((downloadFileUrl, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(downloadFileUrl))
                    {
                        if (File.Exists(MinerTweakTempPath.AtikmdagPatcherFileFullName))
                        {
                            VirtualRoot.Execute(new UnTopmostCommand());
                            Windows.Cmd.RunClose(MinerTweakTempPath.AtikmdagPatcherFileFullName, string.Empty, waitForExit: false);
                        }
                        return;
                    }
                    Uri uri = new Uri(downloadFileUrl);
                    string localVersion = GetAtikmdagPatcherVersion();
                    if (string.IsNullOrEmpty(localVersion) || !File.Exists(MinerTweakTempPath.AtikmdagPatcherFileFullName) || uri.AbsolutePath != localVersion)
                    {
                        VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "下载A卡驱动签名工具", (window, isSuccess, message, saveFileFullName) =>
                        {
                            try
                            {
                                if (isSuccess)
                                {
                                    File.Delete(MinerTweakTempPath.AtikmdagPatcherFileFullName);
                                    File.Move(saveFileFullName, MinerTweakTempPath.AtikmdagPatcherFileFullName);
                                    SetAtikmdagPatcherVersion(uri.AbsolutePath);
                                    window?.Close();
                                    VirtualRoot.Execute(new UnTopmostCommand());
                                    Windows.Cmd.RunClose(MinerTweakTempPath.AtikmdagPatcherFileFullName, string.Empty, waitForExit: false);
                                }
                                else
                                {
                                    VirtualRoot.MyLocalError(nameof(AppRoot), "下载A卡驱动签名工具：" + message, toConsole: true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorDebugLine(ex);
                            }
                        }));
                    }
                    else
                    {
                        VirtualRoot.Execute(new UnTopmostCommand());
                        Windows.Cmd.RunClose(MinerTweakTempPath.AtikmdagPatcherFileFullName, string.Empty, waitForExit: false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorDebugLine(ex);
                }
            });
        }
        #endregion

        #region OpenSwitchRadeonGpu
        private static string GetSwitchRadeonGpuVersion()
        {
            string version = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(LuckyKeyword.SwitchRadeonGpuVersionAppSettingKey, out IAppSetting setting) && setting.Value != null)
            {
                version = setting.Value.ToString();
            }
            return version;
        }

        private static void SetSwitchRadeonGpuVersion(string value)
        {
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData
            {
                Key = LuckyKeyword.SwitchRadeonGpuVersionAppSettingKey,
                Value = value
            }));
        }

        private static void ShowSwitchRadeonGpu(bool on)
        {
            if (on)
            {
                VirtualRoot.MyLocalInfo(nameof(AppRoot), "开启A卡计算模式成功", OutEnum.Success);
            }
            else
            {
                VirtualRoot.MyLocalInfo(nameof(AppRoot), "关闭A卡计算模式成功", OutEnum.Success);
            }
        }

        public static void SwitchRadeonGpu(bool on)
        {
            RpcRoot.OfficialServer.FileUrlService.GetSwitchRadeonGpuUrlAsync((downloadFileUrl, e) =>
            {
                try
                {
                    string args = $"--compute={(on ? "on" : "off")} --admin --restart";
                    if (string.IsNullOrEmpty(downloadFileUrl))
                    {
                        if (File.Exists(MinerTweakTempPath.SwitchRadeonGpuFileFullName))
                        {
                            Windows.Cmd.RunClose(MinerTweakTempPath.SwitchRadeonGpuFileFullName, args, waitForExit: true);
                            ShowSwitchRadeonGpu(on);
                        }
                        return;
                    }
                    Uri uri = new Uri(downloadFileUrl);
                    string localVersion = GetSwitchRadeonGpuVersion();
                    if (string.IsNullOrEmpty(localVersion) || !File.Exists(MinerTweakTempPath.SwitchRadeonGpuFileFullName) || uri.AbsolutePath != localVersion)
                    {
                        VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "下载开启A卡计算模式工具", (window, isSuccess, message, saveFileFullName) =>
                        {
                            try
                            {
                                if (isSuccess)
                                {
                                    File.Delete(MinerTweakTempPath.SwitchRadeonGpuFileFullName);
                                    File.Move(saveFileFullName, MinerTweakTempPath.SwitchRadeonGpuFileFullName);
                                    SetSwitchRadeonGpuVersion(uri.AbsolutePath);
                                    window?.Close();
                                    Windows.Cmd.RunClose(MinerTweakTempPath.SwitchRadeonGpuFileFullName, args, waitForExit: true);
                                    ShowSwitchRadeonGpu(on);
                                }
                                else
                                {
                                    VirtualRoot.MyLocalError(nameof(AppRoot), "下载开启A卡计算模式工具：" + message, toConsole: true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ErrorDebugLine(ex);
                            }
                        }));
                    }
                    else
                    {
                        Windows.Cmd.RunClose(MinerTweakTempPath.SwitchRadeonGpuFileFullName, args, waitForExit: true);
                        ShowSwitchRadeonGpu(on);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorDebugLine(ex);
                }
            });
        }
        #endregion

        #region OpenLiteDb
        public static void OpenLiteDb(string dbFileFullName)
        {
            string liteDbExplorerDir = Path.Combine(MinerTweakTempPath.ToolsDirFullName, "LiteDBExplorerPortable");
            string liteDbExplorerFileFullName = Path.Combine(liteDbExplorerDir, "LiteDbExplorer.exe");
            if (!Directory.Exists(liteDbExplorerDir))
            {
                Directory.CreateDirectory(liteDbExplorerDir);
            }
            if (!File.Exists(liteDbExplorerFileFullName))
            {
                RpcRoot.OfficialServer.FileUrlService.GetLiteDbExplorerUrlAsync((downloadFileUrl, e) =>
                {
                    if (string.IsNullOrEmpty(downloadFileUrl))
                    {
                        return;
                    }
                    VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "LiteDB数据库管理工具", (window, isSuccess, message, saveFileFullName) =>
                    {
                        if (isSuccess)
                        {
                            ZipUtil.DecompressZipFile(saveFileFullName, liteDbExplorerDir);
                            File.Delete(saveFileFullName);
                            window?.Close();
                            Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
                        }
                    }));
                });
            }
            else
            {
                Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
            }
        }
        #endregion

        #region context
        public static MinerProfileViewModel MinerProfileVm
        {
            get
            {
                return MinerProfileViewModel.Instance;
            }
        }

        public static CoinViewModels CoinVms
        {
            get
            {
                return CoinViewModels.Instance;
            }
        }

        public static GpuSpeedViewModels GpuSpeedVms
        {
            get
            {
                return GpuSpeedViewModels.Instance;
            }
        }

        public static StartStopMineButtonViewModel StartStopMineButtonVm
        {
            get
            {
                return StartStopMineButtonViewModel.Instance;
            }
        }

        public static PoolKernelViewModels PoolKernelVms
        {
            get
            {
                return PoolKernelViewModels.Instance;
            }
        }

        public static PackageViewModels PackageVms
        {
            get
            {
                return PackageViewModels.Instance;
            }
        }

        public static CoinGroupViewModels CoinGroupVms
        {
            get
            {
                return CoinGroupViewModels.Instance;
            }
        }

        public static FileWriterViewModels FileWriterVms
        {
            get
            {
                return FileWriterViewModels.Instance;
            }
        }

        public static FragmentWriterViewModels FragmentWriterVms
        {
            get
            {
                return FragmentWriterViewModels.Instance;
            }
        }

        public static CoinKernelViewModels CoinKernelVms
        {
            get
            {
                return CoinKernelViewModels.Instance;
            }
        }

        public static CoinProfileViewModels CoinProfileVms
        {
            get
            {
                return CoinProfileViewModels.Instance;
            }
        }

        public static DriveSetViewModel DriveSetVm
        {
            get
            {
                return DriveSetViewModel.Instance;
            }
        }

        public static GpuProfileViewModels GpuProfileVms
        {
            get
            {
                return GpuProfileViewModels.Instance;
            }
        }

        public static GpuViewModels GpuVms
        {
            get
            {
                return GpuViewModels.Instance;
            }
        }

        public static GroupViewModels GroupVms
        {
            get
            {
                return GroupViewModels.Instance;
            }
        }

        public static KernelInputViewModels KernelInputVms
        {
            get
            {
                return KernelInputViewModels.Instance;
            }
        }

        public static KernelOutputKeywordViewModels KernelOutputKeywordVms
        {
            get
            {
                return KernelOutputKeywordViewModels.Instance;
            }
        }

        public static KernelOutputTranslaterViewModels KernelOutputTranslaterVms
        {
            get
            {
                return KernelOutputTranslaterViewModels.Instance;
            }
        }

        public static KernelOutputViewModels KernelOutputVms
        {
            get
            {
                return KernelOutputViewModels.Instance;
            }
        }

        public static KernelViewModels KernelVms
        {
            get
            {
                return KernelViewModels.Instance;
            }
        }

        public static PoolProfileViewModels PoolProfileVms
        {
            get
            {
                return PoolProfileViewModels.Instance;
            }
        }

        public static PoolViewModels PoolVms
        {
            get
            {
                return PoolViewModels.Instance;
            }
        }

        public static ShareViewModels ShareVms
        {
            get
            {
                return ShareViewModels.Instance;
            }
        }

        public static WalletViewModels WalletVms
        {
            get
            {
                return WalletViewModels.Instance;
            }
        }

        public static SysDicViewModels SysDicVms
        {
            get
            {
                return SysDicViewModels.Instance;
            }
        }

        public static SysDicItemViewModels SysDicItemVms
        {
            get
            {
                return SysDicItemViewModels.Instance;
            }
        }

        public static GpuStatusBarViewModel GpuStatusBarVm
        {
            get
            {
                return GpuStatusBarViewModel.Instance;
            }
        }
        #endregion
    }
}
