﻿using Lucky.Core.MinerTweak;
using Lucky.Hub;
using System;
using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;

namespace Lucky {
    [MessageType(description: "设置本机Ip")]
    public class SetLocalIpCommand : Cmd {
        public SetLocalIpCommand(ILocalIp input, bool isAutoDNSServer) {
            this.Input = input;
            this.IsAutoDNSServer = isAutoDNSServer;
        }

        public ILocalIp Input { get; private set; }
        public bool IsAutoDNSServer { get; private set; }
    }

    [MessageType(description: "本机IP集初始化后")]
    public class LocalIpSetInitedEvent : EventBase {
        public LocalIpSetInitedEvent() { }
    }

    public static partial class VirtualRoot {
        public interface ILocalIpSet {
            LocalIpDto[] AsEnumerable();
        }

        private static readonly ILocalIpSet _localIpSet = new LocalIpSetImpl();
        public static ILocalIpSet LocalIpSet {
            get {
                return _localIpSet;
            }
        }

        public class LocalIpSetImpl : SetBase, ILocalIpSet {
            private LocalIpDto[] _localIps = new LocalIpDto[0];
            public LocalIpSetImpl() {
                NetworkChange.NetworkAddressChanged += (object sender, EventArgs e) => {
                    // 延迟获取网络信息以防止立即获取时获取不到
                    1.SecondsDelay().ContinueWith(t => {
                        var old = _localIps;
                        base.DeferReInit();
                        InitOnce();
                        var localIps = _localIps;
                        if (localIps.Length == 0) {
                            MyLocalWarn(nameof(LocalIpSetImpl), "网络连接已断开", toConsole: true);
                        }
                        else if (old.Length == 0) {
                            MyLocalInfo(nameof(LocalIpSetImpl), "网络连接已连接", toConsole: true);
                        }
                    });
                };
                NetworkChange.NetworkAvailabilityChanged += (object sender, NetworkAvailabilityEventArgs e) => {
                    if (e.IsAvailable) {
                        MyLocalInfo(nameof(LocalIpSetImpl), $"网络可用", toConsole: true);
                    }
                    else {
                        MyLocalWarn(nameof(LocalIpSetImpl), $"网络不可用", toConsole: true);
                    }
                };
                BuildCmdPath<SetLocalIpCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                    #region
                    ManagementObject mo = null;
                    using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                    using (ManagementObjectCollection moc = mc.GetInstances()) {
                        foreach (ManagementObject item in moc) {
                            if ((string)item["SettingID"] == message.Input.SettingID) {
                                mo = item;
                                break;
                            }
                        }
                    }
                    if (mo != null) {
                        if (message.Input.DHCPEnabled) {
                            mo.InvokeMethod("EnableStatic", null);
                            mo.InvokeMethod("SetGateways", null);
                            mo.InvokeMethod("EnableDHCP", null);
                            1.SecondsDelay().ContinueWith(t => {
                                base.DeferReInit();
                                InitOnce();
                            });
                        }
                        else {
                            ManagementBaseObject inPar = mo.GetMethodParameters("EnableStatic");
                            inPar["IPAddress"] = new string[] { message.Input.IPAddress };
                            inPar["SubnetMask"] = new string[] { message.Input.IPSubnet };
                            mo.InvokeMethod("EnableStatic", inPar, null);
                            inPar = mo.GetMethodParameters("SetGateways");
                            inPar["DefaultIPGateway"] = new string[] { message.Input.DefaultIPGateway };
                            mo.InvokeMethod("SetGateways", inPar, null);
                        }

                        if (message.IsAutoDNSServer) {
                            mo.InvokeMethod("SetDNSServerSearchOrder", null);
                        }
                        else {
                            ManagementBaseObject inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                            inPar["DNSServerSearchOrder"] = new string[] { message.Input.DNSServer0, message.Input.DNSServer1 };
                            mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                        }
                    }
                    #endregion
                });
            }

            protected override void Init() {
#if DEBUG
                NTStopwatch.Start();
#endif
                _localIps = GetLocalIps();
                RaiseEvent(new LocalIpSetInitedEvent());
#if DEBUG
                // 将近300毫秒
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    LuckyConsole.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(Init)}");
                }
#endif
            }

            public LocalIpDto[] AsEnumerable() {
                InitOnce();
                return _localIps;
            }

            #region private static methods
            public static IEnumerable<ManagementObject> GetNetCardInfo() {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                using (ManagementObjectCollection moc = mc.GetInstances()) {
                    foreach (ManagementObject mo in moc) {
                        if (!(bool)mo["IPEnabled"] || mo["DefaultIPGateway"] == null) {
                            continue;
                        }
                        string[] defaultIpGateways = (string[])mo["DefaultIPGateway"];
                        if (defaultIpGateways.Length == 0) {
                            continue;
                        }
                        yield return mo;
                    }
                }
            }

            private static LocalIpDto[] GetLocalIps() {
                List<LocalIpDto> list = new List<LocalIpDto>();
                try {
                    foreach (ManagementObject mo in GetNetCardInfo()) {
                        if (!(bool)mo["IPEnabled"] || mo["DefaultIPGateway"] == null) {
                            continue;
                        }
                        string[] defaultIpGateways = (string[])mo["DefaultIPGateway"];
                        if (defaultIpGateways.Length == 0) {
                            continue;
                        }
                        string dNSServer0 = string.Empty;
                        string dNSServer1 = string.Empty;
                        if (mo["DNSServerSearchOrder"] != null) {
                            string[] dNSServerSearchOrder = (string[])mo["DNSServerSearchOrder"];
                            if (dNSServerSearchOrder.Length > 0) {
                                if (dNSServerSearchOrder[0] != defaultIpGateways[0]) {
                                    dNSServer0 = dNSServerSearchOrder[0];
                                }
                            }
                            if (dNSServerSearchOrder.Length > 1) {
                                dNSServer1 = dNSServerSearchOrder[1];
                            }
                        }
                        string ipAddress = string.Empty;
                        if (mo["IPAddress"] != null) {
                            string[] items = (string[])mo["IPAddress"];
                            if (items.Length != 0) {
                                ipAddress = items[0];// 只取Ipv4
                            }
                        }
                        string ipSubnet = string.Empty;
                        if (mo["IPSubnet"] != null) {
                            string[] items = (string[])mo["IPSubnet"];
                            if (items.Length != 0) {
                                ipSubnet = items[0];// 只取Ipv4
                            }
                        }
                        list.Add(new LocalIpDto {
                            DefaultIPGateway = defaultIpGateways[0],
                            DHCPEnabled = (bool)mo["DHCPEnabled"],
                            SettingID = (string)mo["SettingID"],
                            IPSubnet = ipSubnet,
                            DNSServer0 = dNSServer0,
                            DNSServer1 = dNSServer1,
                            IPAddress = ipAddress,
                            MACAddress = (string)mo["MACAddress"]
                        });
                    }
                    FillNames(list);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                return list.ToArray();
            }

            private static void FillNames(List<LocalIpDto> list) {
                //获取网卡
                var items = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var item in list) {
                    foreach (NetworkInterface ni in items) {
                        if (ni.Id == item.SettingID) {
                            item.Name = ni.Name;
                            break;
                        }
                    }
                }
            }
            #endregion
        }
    }
}
