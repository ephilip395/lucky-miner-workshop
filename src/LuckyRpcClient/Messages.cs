using Lucky.Hub;

namespace Lucky {

    [MessageType(description: "切换了群控后台客户端服务类型后")]
    public class MinerMonitorServiceSwitchedEvent : EventBase {
        public MinerMonitorServiceSwitchedEvent(MinerMonitorServiceType serviceType) {
            this.ServiceType = serviceType;
        }

        public MinerMonitorServiceType ServiceType { get; private set; }
    }

    [MessageType(description: "刷新行运矿工程序版本文件集")]
    public class RefreshLuckyFileSetCommand : Cmd {
        public RefreshLuckyFileSetCommand() { }
    }

    [MessageType(description: "行运矿工程序版本文件集初始化后")]
    public class LuckyFileSetInitedEvent : EventBase {
        public LuckyFileSetInitedEvent() { }
    }
}
