using System.ComponentModel;

namespace Lucky.Core.MinerTweak {
    public enum MinerTweakActionType {
        [Description("开启A卡计算模式，如果本机不是A卡将被忽略")]
        SwitchRadeonGpuOn,
        [Description("关闭A卡计算模式，如果本机不是A卡将被忽略")]
        SwitchRadeonGpuOff,
        [Description("禁用Widnows系统更新")]
        BlockWAU
    }
}
