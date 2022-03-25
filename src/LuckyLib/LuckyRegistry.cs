using Microsoft.Win32;
using System;

namespace Lucky
{
    public static partial class LuckyRegistry
    {
        #region 设置Windows开机启动
        /// <summary>
        /// 将当前程序设置为windows开机自动启动
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="isAutoBoot"></param>
        /// <param name="otherParams"></param>
        public static void SetAutoBoot(string valueName, bool isAutoBoot, string otherParams = null)
        {
            const string AutoRunSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            if (isAutoBoot == true)
            {
                string value = VirtualRoot.AppFileFullName;
                if (!string.IsNullOrEmpty(otherParams))
                {
                    value = value + " " + otherParams;
                }
                Windows.WinRegistry.SetValue(Registry.CurrentUser, AutoRunSubKey, valueName, value);
            }
            else
            {
                Windows.WinRegistry.DeleteValue(Registry.CurrentUser, AutoRunSubKey, valueName);
            }
        }
        #endregion

        public const string LuckyRegistrySubKey = @".DEFAULT\Software\Lucky";

        // 下面这些项是可能需要交换到下层系统从而完成不同进程间信息交换的项
        // 注册表就属于下层系统，文件系统也属于下层系统，使用注册表比较简单统一

        private const string MinerMonitor = "MinerMonitor";
        private static string GetValueName(LuckyAppType appType, string baseValueName)
        {
            string valueName;
            switch (appType)
            {
                case LuckyAppType.MinerTweak:
                    valueName = baseValueName;
                    break;
                case LuckyAppType.MinerMonitor:
                    valueName = MinerMonitor + baseValueName;
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return valueName;
        }

        #region Location
        public static string GetLocation(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.LocationRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, valueName);
            if (value != null)
            {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetLocation(LuckyAppType appType, string location)
        {
            string valueName = GetValueName(appType, LuckyKeyword.LocationRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, valueName, location);
        }
        #endregion

        #region Arguments
        public static string GetArguments(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.ArgumentsRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, valueName);
            if (value != null)
            {
                return (string)value;
            }
            return string.Empty;
        }

        public static void SetArguments(LuckyAppType appType, string arguments)
        {
            string valueName = GetValueName(appType, LuckyKeyword.ArgumentsRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, valueName, arguments);
        }
        #endregion

        #region CurrentVersion
        public static string GetCurrentVersion(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.CurrentVersionRegistryKey);
            string currentVersion = "1.0.0.0";
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, valueName);
            if (value != null)
            {
                currentVersion = (string)value;
            }
            if (string.IsNullOrEmpty(currentVersion))
            {
                return "1.0.0.0";
            }
            return currentVersion;
        }

        public static void SetCurrentVersion(LuckyAppType appType, string version)
        {
            string valueName = GetValueName(appType, LuckyKeyword.CurrentVersionRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, valueName, version);
        }
        #endregion

        #region CurrentVersionTag
        public static string GetCurrentVersionTag(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.CurrentVersionTagRegistryKey);
            string currentVersionTag = string.Empty;
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, valueName);
            if (value != null)
            {
                currentVersionTag = (string)value;
            }
            return currentVersionTag;
        }

        public static void SetCurrentVersionTag(LuckyAppType appType, string versionTag)
        {
            string valueName = GetValueName(appType, LuckyKeyword.CurrentVersionTagRegistryKey);
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, valueName, versionTag);
        }
        #endregion

        #region MinerMonitorIsInnerIp
        public static bool GetMinerMonitorIsInnerIp()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, "MinerMonitorIsInnerIp");
            return value != null && value.ToString() == "true";
        }

        public static void SetMinerMonitorIsInnerIp(bool isInnerIp)
        {
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, "MinerMonitorIsInnerIp", isInnerIp ? "true" : "false");
        }
        #endregion

        #region IsNoUi
        public static bool GetIsNoUi()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, "IsNoUi");
            return value != null && value.ToString() == "true";
        }

        public static void SetIsNoUi(bool isNoUi)
        {
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, "IsNoUi", isNoUi ? "true" : "false");
        }
        #endregion

        #region IsAutoStart
        public static bool GetIsAutoStart()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, "IsAutoStart");
            return value != null && value.ToString() == "true";
        }

        public static void SetIsAutoStart(bool isAutoStart)
        {
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, "IsAutoStart", isAutoStart ? "true" : "false");
        }
        #endregion

        #region LoginName
        public static string GetLoginName()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.LoginNameRegistryKey);
            if (value == null)
            {
                value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, "ControlCenterLoginName");
            }
            if (value == null)
            {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetLoginName(string daemonVersion)
        {
            if (daemonVersion == null)
            {
                daemonVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.LoginNameRegistryKey, daemonVersion);
        }
        #endregion

        #region NoDevFeeVersion
        public static string GetNoDevFeeVersion()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.NoDevFeeVersionRegistryKey);
            if (value == null)
            {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetNoDevFeeVersion(string noDevFeeVersion)
        {
            if (noDevFeeVersion == null)
            {
                noDevFeeVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.NoDevFeeVersionRegistryKey, noDevFeeVersion);
        }
        #endregion

        #region DivertProxyNuVersion
        public static string GetDivertProxyNuVersion()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.DivertProxyNuVersionRegistryKey);
            if (value == null)
            {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetDivertProxyNuVersion(string divertProxyNuVersion)
        {
            if (divertProxyNuVersion == null)
            {
                divertProxyNuVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.DivertProxyNuVersionRegistryKey, divertProxyNuVersion);
        }
        #endregion

        #region DaemonVersion
        public static string GetDaemonVersion()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.DaemonVersionRegistryKey);
            if (value == null)
            {
                return string.Empty;
            }
            return (string)value;
        }

        public static void SetDaemonVersion(string daemonVersion)
        {
            if (daemonVersion == null)
            {
                daemonVersion = string.Empty;
            }
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.DaemonVersionRegistryKey, daemonVersion);
        }
        #endregion

        #region GetClientId
        public static Guid GetClientId(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.ClientIdRegistryKey);
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, valueName);
            if (value == null || !Guid.TryParse((string)value, out Guid id))
            {
                id = Guid.NewGuid();
                Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, valueName, id.ToString());
            }
            return id;
        }
        #endregion

        #region ReClientId
        public static void ReClientId(LuckyAppType appType)
        {
            string valueName = GetValueName(appType, LuckyKeyword.ClientIdRegistryKey);
            Windows.WinRegistry.DeleteValue(Registry.Users, LuckyRegistrySubKey, valueName);
        }
        #endregion

        #region GetIsRdpEnabled
        public static bool GetIsRdpEnabled()
        {
            try
            {
                return (int)Windows.WinRegistry.GetValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Control\\Terminal Server", "fDenyTSConnections") == 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region SetIsRdpEnabled
        public static void SetIsRdpEnabled(bool enabled)
        {
            if (enabled)
            {
                SetRdpRegistryValue(0);
            }
            else
            {
                SetRdpRegistryValue(1);
            }
        }

        #region private SetRdpRegistryValue
        private static void SetRdpRegistryValue(int value)
        {
            try
            {
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default),
                               rdpKey = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", true))
                {
                    if (!int.TryParse(rdpKey.GetValue("fDenyTSConnections").ToString(), out int currentValue))
                    {
                        currentValue = -1;
                    }

                    //Value was not found do not proceed with change.
                    if (currentValue == -1)
                    {
                        return;
                    }
                    else if (value == 1 && currentValue == 1)
                    {
                        LuckyConsole.DevDebug("RDP is already disabled. No changes will be made.");
                        return;
                    }
                    else if (value == 0 && currentValue == 0)
                    {
                        LuckyConsole.DevDebug("RDP is already enabled. No changes will be made.");
                        return;
                    }
                    else
                    {
                        rdpKey.SetValue("fDenyTSConnections", value);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #endregion
    }
}
