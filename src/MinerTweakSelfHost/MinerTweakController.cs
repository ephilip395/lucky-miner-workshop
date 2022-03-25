using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.Report;
using Lucky.Ws;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Lucky
{
    /// <summary>
    /// 端口号：<see cref="LuckyKeyword.MinerTweakPort"/>
    /// </summary>
    public class MinerTweakController : ApiController, IMinerTweakController
    {
        [HttpPost]
        public bool ShowMainWindow()
        {
            try
            {
                VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: false));
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseLucky([FromBody] object request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            try
            {
                100.MillisecondsDelay().ContinueWith((t) =>
                {
                    VirtualRoot.Execute(new CloseLuckyCommand("挖矿端升级后关闭旧版挖矿端"));
                });
                return ResponseBase.Ok();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody] WorkRequest request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            try
            {
                VirtualRoot.MyLocalInfo(nameof(MinerTweakController), $"通过群控开始挖矿", toConsole: true);
                WorkType workType = WorkType.None;
                if (request.WorkId != Guid.Empty)
                {
                    workType = WorkType.MineWork;
                    if (request.WorkId.IsSelfMineWorkId())
                    {
                        workType = WorkType.SelfWork;
                    }
                }
                LuckyContext.Instance.RestartMine(workType, request.WorkerName);
                return ResponseBase.Ok();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody] object request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            try
            {
                VirtualRoot.MyLocalInfo(nameof(MinerTweakController), "通过群控停止挖矿", toConsole: true);
                VirtualRoot.Execute(new StopMineCommand());
                return ResponseBase.Ok();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        [HttpGet]
        public SpeedDto GetSpeed()
        {
            try
            {
                return LuckyContext.Instance.ReporterDataProvider.CreateSpeedDto();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpPost]
        [HttpGet]
        public SpeedDto WsGetSpeed()
        {
            try
            {
                var dataProvider = LuckyContext.Instance.ReporterDataProvider;
                dataProvider.WsGetSpeedOn = DateTime.Now;
                return dataProvider.CreateSpeedDto();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return null;
            }
        }

        [HttpGet]
        [HttpPost]
        public List<ConsoleOutLine> GetConsoleOutLines(long afterTime)
        {
            return LuckyConsole.ConsoleOutLineSet.Gets(afterTime);
        }

        [HttpGet]
        [HttpPost]
        public List<LocalMessageDto> GetLocalMessages(long afterTime)
        {
            return LuckyContext.Instance.LocalMessageSet.LocalMessageDtoSet.Gets(afterTime);
        }

        [HttpPost]
        public void RefreshAutoBootStart()
        {
            VirtualRoot.Execute(new RefreshAutoBootStartCommand());
        }

        [HttpPost]
        public void RefreshIsRemoteDesktopEnabled()
        {
            VirtualRoot.Execute(new RefreshIsRemoteDesktopEnabledCommand());
        }

        [HttpPost]
        public void ReportWsDaemonState([FromBody] WsClientState state)
        {
            VirtualRoot.Execute(new RefreshWsStateCommand(state));
        }

        [HttpPost]
        public void OverClock()
        {
            VirtualRoot.MyLocalInfo(nameof(MinerTweakController), $"通过群控刷新超频", toConsole: true);
            LuckyContext.Instance.GpuProfileSet.RemoteOverClock();
        }

        [HttpPost]
        public void RunAction([FromBody] DataRequest<MinerTweakActionType> request)
        {
            if (request == null)
            {
                return;
            }
            VirtualRoot.MyLocalInfo(nameof(MinerTweakController), request.Data.GetDescription(), toConsole: true);
            VirtualRoot.Execute(new MinerTweakActionCommand(request.Data));
        }
    }
}
