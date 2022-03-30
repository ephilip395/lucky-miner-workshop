using Lucky.Core;
using Lucky.MinerMonitor.Vms;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lucky.MinerMonitor
{
    public static partial class MinerMonitorRoot
    {
        public class MinerTweakConsoleViewModel : ViewModelBase
        {
            private readonly List<ConsoleOutLine> _outLines = new List<ConsoleOutLine>();
            private readonly object _locker = new object();
            private MinerViewModel _minerClientVm;
            private DateTime _latestTimestamp = Timestamp.UnixBaseTime;

            public DateTime LatestTimestamp
            {
                get { return _latestTimestamp; }
                set
                {
                    if (_latestTimestamp != value)
                    {
                        _latestTimestamp = value;
                        OnPropertyChanged(nameof(LatestTimestamp));
                        OnPropertyChanged(nameof(LatestTimeSpanText));
                    }
                }
            }

            public string LatestTimeSpanText
            {
                get
                {
                    if (LatestTimestamp == Timestamp.UnixBaseTime)
                    {
                        return string.Empty;
                    }
                    return Timestamp.GetTimeSpanBeforeText(LatestTimestamp);
                }
            }

            public MinerTweakConsoleViewModel()
            {
                if (ClientAppType.IsMinerMonitor)
                {
                    VirtualRoot.BuildEventPath<MinerTweakSelectionChangedEvent>("刷新矿机控制台输出", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                    {
                        bool isChanged = true;
                        if (message.MinerTweakVm != null && this._minerClientVm != null && this._minerClientVm.ClientId == message.MinerTweakVm.ClientId)
                        {
                            isChanged = false;
                        }
                        LatestTimestamp = Timestamp.UnixBaseTime;
                        if (isChanged)
                        {
                            lock (_locker)
                            {
                                _outLines.Clear();
                                try
                                {
                                    Console.Clear();
                                }
                                catch
                                {
                                }
                                this._minerClientVm = message.MinerTweakVm;
                            }
                            SendGetConsoleOutLinesMqMessage(isFast: true);
                        }
                    });
                    VirtualRoot.BuildEventPath<ClientConsoleOutLinesEvent>("将收到的挖矿端控制台消息输出到输出窗口", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                    {
                        if (this._minerClientVm == null
                            || this._minerClientVm.ClientId != message.ClientId
                            || message.Data == null
                            || message.Data.Count == 0)
                        {
                            return;
                        }
                        lock (_locker)
                        {
                            foreach (var item in message.Data)
                            {
                                _outLines.Add(item);
                                LuckyConsole.UserLine(item.Line, ConsoleColor.White, withPrefix: false);
                            }
                            // 因为客户端的时间可能不准所以不能使用客户端的时间
                            LatestTimestamp = DateTime.Now;
                        }
                    });
                    VirtualRoot.BuildEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的控制台输出", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message =>
                    {
                        SendGetConsoleOutLinesMqMessage(isFast: false);
                    });
                    VirtualRoot.BuildEventPath<Per1SecondEvent>("客户端控制台输出倒计时秒表", LogEnum.None, this.GetType(), PathPriority.Normal, path: message =>
                    {
                        if (this._minerClientVm == null || this._latestTimestamp == Timestamp.UnixBaseTime)
                        {
                            return;
                        }
                        OnPropertyChanged(nameof(LatestTimeSpanText));
                    });
                }
            }

            private void SendGetConsoleOutLinesMqMessage(bool isFast)
            {
                if (this._minerClientVm == null)
                {
                    return;
                }
                long afterTime = 0;
                var minerClientVm = this._minerClientVm;
                lock (_locker)
                {
                    var item = _outLines.LastOrDefault();
                    if (item != null)
                    {
                        afterTime = item.Timestamp;
                    }
                }
                if (isFast)
                {
                    MinerMonitorService.FastGetConsoleOutLinesAsync(minerClientVm, afterTime);
                }
                else
                {
                    MinerMonitorService.GetConsoleOutLinesAsync(minerClientVm, afterTime);
                }
            }
        }
    }
}
