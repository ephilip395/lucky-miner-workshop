﻿using NetFwTypeLib;
using Lucky.Windows;
using System;
using System.Linq;

namespace Lucky.RemoteDesktop {
    public enum FirewallDomain {
        Domain = 0x0001,
        Private = 0x0002,
        Public = 0x0004
    }

    public enum FirewallStatus {
        Enabled = 1,
        Disabled = 0
    }

    public static class Firewall {
        private const int RdpTcpPort = 3389;
        private const int RdpUdpPort = 3389;
        private const NET_FW_SCOPE_ RdpScope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
        private const string RdpRuleName = "RDPEnabler";

        private const NET_FW_SCOPE_ MinerTweakScope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
        private const string MinerTweakRuleName = "MinerTweak";
        private const string LuckyDaemonRuleName = "LuckyDaemon";

        #region DisableFirewall
        public static bool DisableFirewall() {
            FirewallStatus state = Status(FirewallDomain.Domain);
            if (state == RemoteDesktop.FirewallStatus.Disabled) {
                return true;
            }
            try {
                int exitcode = -1;
                Cmd.RunClose("netsh", "advfirewall set allprofiles state off", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Logger.OkDebugLine("disable firewall ok");
                }
                else {
                    Logger.WarnDebugLine("disable firewall failed, exitcode=" + exitcode);
                }
                return r;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("disable firewall failed，因为异常", e);
                return false;
            }
        }
        #endregion

        #region EnableFirewall
        /// <summary>
        /// 仅供单元测试
        /// </summary>
        public static bool EnableFirewall() {
            FirewallStatus state = Status(FirewallDomain.Domain);
            if (state == RemoteDesktop.FirewallStatus.Enabled) {
                return true;
            }
            try {
                int exitcode = -1;
                Cmd.RunClose("netsh", "advfirewall set allprofiles state on", ref exitcode);
                bool r = exitcode == 0;
                if (r) {
                    Logger.OkDebugLine("enable firewall ok");
                }
                else {
                    Logger.WarnDebugLine("enable firewall failed, exitcode=" + exitcode);
                }
                return r;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine("enable firewall failed，因为异常", e);
                return false;
            }
        }
        #endregion

        public static FirewallStatus Status(FirewallDomain? domain = null) {
            try {
                return FirewallStatus(domain);
            }
            catch {
                return RemoteDesktop.FirewallStatus.Disabled;
            }
        }

        public static void AddMinerTweakRule() {
            try {
                OpenPort($"{MinerTweakRuleName}_TCP", LuckyKeyword.MinerTweakPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP, MinerTweakScope);
                OpenPort($"{MinerTweakRuleName}_UDP", LuckyKeyword.MinerTweakPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP, MinerTweakScope);

                OpenPort($"{LuckyDaemonRuleName}_TCP", LuckyKeyword.LuckyDaemonPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP, MinerTweakScope);
                OpenPort($"{LuckyDaemonRuleName}_UDP", LuckyKeyword.LuckyDaemonPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP, MinerTweakScope);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 仅供单元测试
        /// </summary>
        public static void RemoveMinerTweakRule() {
            try {
                INetFwOpenPorts openPorts = GetOpenPorts();
                openPorts.Remove(LuckyKeyword.MinerTweakPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
                openPorts.Remove(LuckyKeyword.MinerTweakPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);

                openPorts.Remove(LuckyKeyword.LuckyDaemonPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
                openPorts.Remove(LuckyKeyword.LuckyDaemonPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);

                INetFwPolicy2 policyManager = GetPolicyManager();
                policyManager.Rules.Remove(MinerTweakRuleName);
                policyManager.Rules.Remove(LuckyDaemonRuleName);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public static bool IsMinerTweakRuleExists() {
            try {
                INetFwPolicy2 policyManager = GetPolicyManager();
                return
                    policyManager.Rules.OfType<INetFwRule>().Any(x => x.Name.StartsWith(MinerTweakRuleName)) &&
                    policyManager.Rules.OfType<INetFwRule>().Any(x => x.Name.StartsWith(LuckyDaemonRuleName));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return true;
            }
        }

        public static void AddRdpRule() {
            try {
                OpenPort($"{RdpRuleName}_TCP", RdpTcpPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP, RdpScope);
                OpenPort($"{RdpRuleName}_UDP", RdpUdpPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP, RdpScope);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 仅供单元测试
        /// </summary>
        public static void RemoveRdpRule() {
            try {
                INetFwOpenPorts openPorts = GetOpenPorts();
                openPorts.Remove(RdpTcpPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
                openPorts.Remove(RdpUdpPort, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);

                INetFwPolicy2 policyManager = GetPolicyManager();
                policyManager.Rules.Remove(RdpRuleName);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        /// <summary>
        /// 仅供单元测试
        /// </summary>
        public static bool IsRdpRuleExists() {
            try {
                INetFwPolicy2 policyManager = GetPolicyManager();
                return policyManager.Rules.OfType<INetFwRule>().Any(x => x.Name.StartsWith(RdpRuleName));
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return true;
            }
        }

        #region private methods
        private static FirewallStatus FirewallStatus(FirewallDomain? domain) {
            // Gets the current firewall profile (domain, public, private, etc.)
            NET_FW_PROFILE_TYPE2_ fwCurrentProfileTypes;

            INetFwPolicy2 policyManager = GetPolicyManager();
            if (domain.HasValue) {
                fwCurrentProfileTypes = (NET_FW_PROFILE_TYPE2_)domain;
            }
            else {
                fwCurrentProfileTypes = (NET_FW_PROFILE_TYPE2_)policyManager.CurrentProfileTypes;
            }

            return (FirewallStatus)Convert.ToInt32(policyManager.get_FirewallEnabled(fwCurrentProfileTypes));
        }

        private static void OpenPort(string name, int port, NET_FW_IP_PROTOCOL_ protocol, NET_FW_SCOPE_ scope) {
            INetFwOpenPorts openPorts = GetOpenPorts();
            if (openPorts.OfType<INetFwOpenPort>().Where(x => x.Name == name).Count() == 0) {
                INetFwOpenPort openPort = (INetFwOpenPort)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwOpenPort"));
                openPort.Port = port;
                openPort.Protocol = protocol;
                openPort.Scope = scope;
                openPort.Name = name;

                openPorts.Add(openPort);
            }
        }

        private static INetFwPolicy2 GetPolicyManager() {
            INetFwPolicy2 policyManager = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            return policyManager;
        }

        private static INetFwOpenPorts GetOpenPorts() {
            INetFwMgr manager = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
            INetFwProfile profile = manager.LocalPolicy.CurrentProfile;
            INetFwOpenPorts openPorts = profile.GloballyOpenPorts;
            return openPorts;
        }
        #endregion
    }
}
