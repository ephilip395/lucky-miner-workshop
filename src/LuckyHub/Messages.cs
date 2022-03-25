﻿using Lucky.Hub;

namespace Lucky
{
    [MessageType(description: "操作系统注销或关闭时")]
    public class OsSessionEndingEvent : EventBase
    {
        public enum ReasonSessionEnding
        {
            Logoff,
            Shutdown,
            Unknown
        }

        public OsSessionEndingEvent(ReasonSessionEnding sessionEndingReason)
        {
            this.SessionEndingReason = sessionEndingReason;
        }

        public ReasonSessionEnding SessionEndingReason { get; private set; }
    }

    [MessageType(description: "程序退出时")]
    public class AppExitEvent : EventBase
    {
        public AppExitEvent() { }
    }

    [MessageType(description: "已经启动1秒钟", isCanNoPath: true)]
    public class HasBoot1SecondEvent : EventBase
    {
        public readonly int Seconds = 1;

        internal HasBoot1SecondEvent() { }
    }

    [MessageType(description: "已经启动2秒钟", isCanNoPath: true)]
    public class HasBoot2SecondEvent : EventBase
    {
        public readonly int Seconds = 2;

        internal HasBoot2SecondEvent() { }
    }

    [MessageType(description: "已经启动5秒钟", isCanNoPath: true)]
    public class HasBoot5SecondEvent : EventBase
    {
        public readonly int Seconds = 5;

        internal HasBoot5SecondEvent() { }
    }

    [MessageType(description: "已经启动10秒钟", isCanNoPath: true)]
    public class HasBoot10SecondEvent : EventBase
    {
        public readonly int Seconds = 10;

        internal HasBoot10SecondEvent() { }
    }

    [MessageType(description: "已经启动20秒钟", isCanNoPath: true)]
    public class HasBoot20SecondEvent : EventBase
    {
        public readonly int Seconds = 20;

        internal HasBoot20SecondEvent() { }
    }

    [MessageType(description: "已经启动1分钟", isCanNoPath: true)]
    public class HasBoot1MinuteEvent : EventBase
    {
        public readonly int Seconds = 60;

        internal HasBoot1MinuteEvent() { }
    }

    [MessageType(description: "已经启动2分钟", isCanNoPath: true)]
    public class HasBoot2MinuteEvent : EventBase
    {
        public readonly int Seconds = 120;

        internal HasBoot2MinuteEvent() { }
    }

    [MessageType(description: "已经启动5分钟", isCanNoPath: true)]
    public class HasBoot5MinuteEvent : EventBase
    {
        public readonly int Seconds = 300;

        internal HasBoot5MinuteEvent() { }
    }

    [MessageType(description: "已经启动10分钟", isCanNoPath: true)]
    public class HasBoot10MinuteEvent : EventBase
    {
        public readonly int Seconds = 600;

        internal HasBoot10MinuteEvent() { }
    }

    [MessageType(description: "已经启动20分钟", isCanNoPath: true)]
    public class HasBoot20MinuteEvent : EventBase
    {
        public readonly int Seconds = 1200;

        internal HasBoot20MinuteEvent() { }
    }

    [MessageType(description: "已经启动50分钟", isCanNoPath: true)]
    public class HasBoot50MinuteEvent : EventBase
    {
        public readonly int Seconds = 3000;

        internal HasBoot50MinuteEvent() { }
    }

    [MessageType(description: "已经启动100分钟", isCanNoPath: true)]
    public class HasBoot100MinuteEvent : EventBase
    {
        public readonly int Seconds = 6000;

        internal HasBoot100MinuteEvent() { }
    }

    [MessageType(description: "已经启动24小时", isCanNoPath: true)]
    public class HasBoot24HourEvent : EventBase
    {
        public readonly int Seconds = 86400;

        internal HasBoot24HourEvent() { }
    }

    [MessageType(description: "每1秒事件", isCanNoPath: true)]
    public class Per1SecondEvent : EventBase
    {
        public readonly int Seconds = 1;

        internal Per1SecondEvent() { }
    }

    [MessageType(description: "每2秒事件", isCanNoPath: true)]
    public class Per2SecondEvent : EventBase
    {
        public readonly int Seconds = 2;

        internal Per2SecondEvent() { }
    }

    [MessageType(description: "每5秒事件", isCanNoPath: true)]
    public class Per5SecondEvent : EventBase
    {
        public readonly int Seconds = 5;

        internal Per5SecondEvent() { }
    }

    [MessageType(description: "每10秒事件", isCanNoPath: true)]
    public class Per10SecondEvent : EventBase
    {
        public readonly int Seconds = 10;

        internal Per10SecondEvent() { }
    }

    [MessageType(description: "每20秒事件", isCanNoPath: true)]
    public class Per20SecondEvent : EventBase
    {
        public readonly int Seconds = 20;

        internal Per20SecondEvent() { }
    }

    [MessageType(description: "时间的分钟部分变更后", isCanNoPath: true)]
    public class MinutePartChangedEvent : EventBase
    {
        internal MinutePartChangedEvent() { }
    }

    [MessageType(description: "每1分钟事件", isCanNoPath: true)]
    public class Per1MinuteEvent : EventBase
    {
        public readonly int Seconds = 60;

        internal Per1MinuteEvent() { }
    }

    [MessageType(description: "每2分钟事件", isCanNoPath: true)]
    public class Per2MinuteEvent : EventBase
    {
        public readonly int Seconds = 120;

        internal Per2MinuteEvent() { }
    }

    [MessageType(description: "每5分钟事件", isCanNoPath: true)]
    public class Per5MinuteEvent : EventBase
    {
        public readonly int Seconds = 300;

        internal Per5MinuteEvent() { }
    }

    [MessageType(description: "每10分钟事件", isCanNoPath: true)]
    public class Per10MinuteEvent : EventBase
    {
        public readonly int Seconds = 600;

        internal Per10MinuteEvent() { }
    }

    [MessageType(description: "每20分钟事件", isCanNoPath: true)]
    public class Per20MinuteEvent : EventBase
    {
        public readonly int Seconds = 1200;
        internal Per20MinuteEvent() { }
    }

    [MessageType(description: "每50分钟事件", isCanNoPath: true)]
    public class Per50MinuteEvent : EventBase
    {
        public readonly int Seconds = 3000;
        internal Per50MinuteEvent() { }
    }

    [MessageType(description: "每100分钟事件", isCanNoPath: true)]
    public class Per100MinuteEvent : EventBase
    {
        public readonly int Seconds = 6000;
        internal Per100MinuteEvent() { }
    }

    [MessageType(description: "每24小时事件", isCanNoPath: true)]
    public class Per24HourEvent : EventBase
    {
        public readonly int Seconds = 86400;
        internal Per24HourEvent() { }
    }

    [MessageType(description: "新的一天到来了（刚过24点，0点0分1秒的时候）", isCanNoPath: true)]
    public class NewDayEvent : EventBase
    {
        internal NewDayEvent() { }
    }
}
