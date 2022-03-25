using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.Report;
using Lucky.Ws;
using System.Collections.Generic;

namespace Lucky.Controllers
{
    /// <summary>
    /// 挖矿端的矿机操作接口
    /// </summary>
    public interface IMinerTweakController
    {
        bool ShowMainWindow();
        ResponseBase CloseLucky(object request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(object request);
        SpeedDto GetSpeed();
        SpeedDto WsGetSpeed();
        List<ConsoleOutLine> GetConsoleOutLines(long afterTime);
        List<LocalMessageDto> GetLocalMessages(long afterTime);
        void RefreshAutoBootStart();
        void RefreshIsRemoteDesktopEnabled();
        void ReportWsDaemonState(WsClientState state);
        void OverClock();
        void RunAction(DataRequest<MinerTweakActionType> request);
    }
}
