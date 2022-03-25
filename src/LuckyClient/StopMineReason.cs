﻿using System.ComponentModel;

namespace Lucky {
    /// <summary>
    /// 最近一次停止挖矿的原因
    /// </summary>
    public enum StopMineReason {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown,
        /// <summary>
        /// 用户点击停止按钮
        /// </summary>
        [Description("用户点击停止按钮")]
        LocalUserAction,
        /// <summary>
        /// 开始挖矿时前保障，真正开始挖矿前做一次停止挖矿操作
        /// </summary>
        [Description("开始挖矿时前保障，真正开始挖矿前做一次停止挖矿操作")]
        InStartMine,
        /// <summary>
        /// CPU温度过高
        /// </summary>
        [Description("CPU温度过高")]
        HighCpuTemperature,
        /// <summary>
        /// GPU温度过高
        /// </summary>
        [Description("GPU温度过高")]
        HighGpuTemperature,
        /// <summary>
        /// 用户通过群控远程停止挖矿
        /// </summary>
        [Description("用户通过群控远程停止挖矿")]
        RPCUserAction,
        /// <summary>
        /// 挖矿内核进程消失
        /// </summary>
        [Description("挖矿内核进程消失")]
        KernelProcessLost,
        /// <summary>
        /// 退出行运矿工
        /// </summary>
        [Description("退出行运矿工")]
        ApplicationExit
    }
}
