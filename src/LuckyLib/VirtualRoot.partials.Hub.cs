﻿using Lucky.Hub;
using Lucky.Timing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lucky
{
    public static partial class VirtualRoot
    {
        // 视图层有个界面提供给开发者观察系统的消息路径情况所以是public的。
        // 系统根上的一些状态集的构造时最好都放在MessageHub初始化之后，因为状态集的构造
        // 函数中可能会建造消息路径，所以这里保证在访问MessageHub之前一定完成了构造。
        public static readonly IMessagePathHub MessageHub = new MessagePathHub();

        private static ITimingEventProducer _timingEventProducer = null;
        public static void StartTimer(ITimingEventProducer timingEventProducer = null)
        {
            if (_timingEventProducer != null)
            {
                throw new InvalidProgramException("秒表已经启动，不能重复启动");
            }
            if (timingEventProducer == null)
            {
                timingEventProducer = new DefaultTimingEventProducer(MessageHub);
            }
            _timingEventProducer = timingEventProducer;
            timingEventProducer.Start();
        }

        public static void RaiseEvent<TEvent>(TEvent evnt) where TEvent : class, IEvent
        {
            MessageHub.Route(evnt);
        }

        public static void Execute<TCmd>(TCmd command) where TCmd : class, ICmd
        {
            MessageHub.Route(command);
        }

        /// <summary>
        /// 修建消息的运动路径
        /// </summary>
        public static IMessagePathId BuildMessagePath<TMessage>(string description, LogEnum logType, Type location, PathPriority priority, Action<TMessage> path)
        {
            return MessageHub.AddPath(location.FullName, description, logType, pathId: PathId.Empty, priority, path);
        }

        /// <summary>
        /// 消息通过路径一次后路径即消失。
        /// 注意该路径具有特定的路径标识pathId，pathId可以看作是路径的形状，只有和该路径的形状相同的消息才能通过路径。
        /// </summary>
        public static IMessagePathId BuildOncePath<TMessage>(string description, LogEnum logType, PathId pathId, Type location, PathPriority priority, Action<TMessage> path)
        {
            return MessageHub.AddPath(location.FullName, description, logType, pathId, priority, path, viaTimesLimit: 1);
        }

        /// <summary>
        /// 消息通过路径指定的次数后路径即消失
        /// </summary>
        public static IMessagePathId BuildViaTimesLimitPath<TMessage>(string description, LogEnum logType, int viaTimesLimit, Type location, PathPriority priority, Action<TMessage> path)
        {
            return MessageHub.AddPath(location.FullName, description, logType, pathId: PathId.Empty, priority, path, viaTimesLimit);
        }

        /// <summary>
        /// 消息通过路径指定的次数后路径即消失
        /// </summary>
        public static IMessagePathId BuildViaTimesLimitPath<TMessage>(string description, LogEnum logType, int viaTimesLimit, string location, PathPriority priority, Action<TMessage> path)
        {
            return MessageHub.AddPath(location, description, logType, pathId: PathId.Empty, priority, path, viaTimesLimit);
        }

        public static IMessagePathId BuildCmdPath<TCmd>(Type location, LogEnum logType, Action<TCmd> path)
            where TCmd : ICmd
        {
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeAttribute(typeof(TCmd));
            return BuildMessagePath($"处理{messageTypeDescription.Description}命令", logType, location, PathPriority.Normal, path);
        }

        public static IMessagePathId BuildEventPath<TEvent>(string description, LogEnum logType, Type location, PathPriority priority, Action<TEvent> path)
            where TEvent : IEvent
        {
            return BuildMessagePath(description, logType, location, priority, path);
        }

        public static void RemoveMessagePath(IMessagePathId pathId)
        {
            if (pathId == null)
            {
                return;
            }
            MessageHub.RemovePath(pathId);
        }

        private static readonly Dictionary<string, Regex> _regexDic = new Dictionary<string, Regex>();
        // 【性能】缓存构建的正则对象
        public static Regex GetRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return null;
            }
            if (_regexDic.TryGetValue(pattern, out Regex regex))
            {
                return regex;
            }
            lock (_locker)
            {
                if (!_regexDic.TryGetValue(pattern, out regex))
                {
                    regex = new Regex(pattern, RegexOptions.Compiled);
                    _regexDic.Add(pattern, regex);
                }
                return regex;
            }
        }
    }
}
