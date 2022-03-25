﻿namespace Lucky.Core.MinerTweak {
    public class LocalIpDto : ILocalIp {
        public LocalIpDto() {
        }

        public string SettingID { get; set; }

        public string Name { get; set; }

        public string DefaultIPGateway { get; set; }

        public bool DHCPEnabled { get; set; }

        public string IPAddress { get; set; }

        public string MACAddress { get; set; }

        public string IPSubnet { get; set; }

        public string DNSServer0 { get; set; }

        public string DNSServer1 { get; set; }

        public static bool operator ==(LocalIpDto left, LocalIpDto right) {
            if (ReferenceEquals(left, right)) {
                return true;
            }
            if ((left is null) || (right is null)) {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(LocalIpDto left, LocalIpDto right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (!(obj is LocalIpDto data)) {
                return false;
            }

            return this.ToString() == data.ToString();
        }

        public bool Equals(LocalIpDto obj) {
            if (obj is null) {
                return false;
            }

            return this.ToString() == obj.ToString();
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        public override string ToString() {
            return
$@"SettingID={SettingID}
Name={Name}
DefaultIPGateway={DefaultIPGateway}
DHCPEnabled={DHCPEnabled}
IPAddress={IPAddress}
MACAddress={MACAddress}
IPSubnet={IPSubnet}
DNSServer0={DNSServer0}
DNSServer1={DNSServer1}";
        }
    }
}
