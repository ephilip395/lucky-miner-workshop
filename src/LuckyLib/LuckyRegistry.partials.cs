using Microsoft.Win32;
using Lucky.Core;

namespace Lucky
{
    public static partial class LuckyRegistry
    {
        #region IsOuterUserEnabled
        public static bool GetIsOuterUserEnabled()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.IsOuterUserEnabledRegistryKey);
            if (value == null)
            {
                return false;
            }
            string str = value.ToString();
            if (!bool.TryParse(str, out bool isOuterUserEnabled))
            {
                return false;
            }
            return isOuterUserEnabled;
        }
        #endregion

        #region GetOuterUserId
        public static string GetOuterUserId()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.OuterUserIdRegistryKey);
            return (value ?? string.Empty).ToString();
        }
        #endregion

        public static bool GetIsUseProxy()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.IsUseProxyRegistryKey);
            if (value == null)
            {
                return false;
            }
            string str = value.ToString();
            if (!bool.TryParse(str, out bool isUseProxy))
            {
                return false;
            }
            return isUseProxy;
        }

        public static string GetProxyServerAddress()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.ProxyServerAddressRegistryKey);
            return (value ?? string.Empty).ToString();
        }

        public static int GetProxyServerPort()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.ProxyServerPortRegistryKey);
            if (value == null)
            {
                return 0;
            }
            string str = value.ToString();
            if (!int.TryParse(str, out int proxyServerPort))
            {
                return 0;
            }
            return proxyServerPort;
        }

        public static bool GetIsProxyPool()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.IsProxyPoolRegistryKey);
            if (value == null)
            {
                return false;
            }
            string str = value.ToString();
            if (!bool.TryParse(str, out bool isProxyPool))
            {
                return false;
            }
            return isProxyPool;
        }

        public static bool GetIsProxyGroupMonitor()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.IsProxyGroupMonitorRegistryKey);
            if (value == null)
            {
                return false;
            }
            string str = value.ToString();
            if (!bool.TryParse(str, out bool isProxyGroupMonitor))
            {
                return false;
            }
            return isProxyGroupMonitor;
        }

        #region WorkType
        public static WorkType GetWorkType()
        {
            object value = Windows.WinRegistry.GetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.WorkTypeRegistryKey);
            return value != null && value.ToString().TryParse(out WorkType workType) ? workType : WorkType.None;
        }

        public static void SetWorkType(WorkType workType)
        {
            Windows.WinRegistry.SetValue(Registry.Users, LuckyRegistrySubKey, LuckyKeyword.WorkTypeRegistryKey, workType.ToString());
        }
        #endregion

        public static string GetMinerTweakArguments(LuckyAppType appType)
        {
            string arguments = GetArguments(appType);
            if (appType == LuckyAppType.MinerTweak)
            {
                switch (GetWorkType())
                {
                    case WorkType.None:
                        break;
                    case WorkType.SelfWork:
                        arguments = "--selfWork " + arguments;
                        break;
                    case WorkType.MineWork:
                        arguments = "--work " + arguments;
                        break;
                    default:
                        break;
                }
            }
            return arguments;
        }
    }
}
