using Lucky.Controllers;
using Lucky.Core;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.VirtualMemory;
using Lucky.Ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lucky
{
    /// <summary>
    /// 端口号：<see cref="LuckyKeyword.LuckyDaemonPort"/>
    /// </summary>
    public class LuckyDaemonController : ApiController, ILuckyDaemonController
    {
        [HttpPost]
        public DataResponse<IntPtr> ShowConsole()
        {
            if (!LuckyConsole.Enabled)
            {
                LuckyConsole.Enabled = true;
                LuckyConsole.UserOk("打开控制台窗口");
                Console.Title = "行运矿工守护程序";
            }
            IntPtr intPtr = LuckyConsole.GetOrAlloc();
            return DataResponse<IntPtr>.Ok(intPtr);
        }

        [HttpGet]
        [HttpPost]
        public WsClientState GetWsDaemonState()
        {
            return VirtualRoot.DaemonWsClient.GetState();
        }

        [HttpPost]
        public void ReClientId(Guid newClientId)
        {
            VirtualRoot.DaemonWsClient.OnClientIdChanged(newClientId);
        }

        [HttpPost]
        public ResponseBase EnableRemoteDesktop()
        {
            return VirtualRoot.DaemonOperation.EnableRemoteDesktop();
        }

        [HttpPost]
        public ResponseBase BlockWAU()
        {
            return VirtualRoot.DaemonOperation.BlockWAU();
        }

        [HttpPost]
        public ResponseBase SwitchRadeonGpu(bool on)
        {
            return VirtualRoot.DaemonOperation.SwitchRadeonGpu(on);
        }

        [HttpPost]
        public void CloseDaemon()
        {
            VirtualRoot.DaemonOperation.CloseDaemon();
        }

        [HttpPost]
        public string GetSelfWorkLocalJson()
        {
            return VirtualRoot.DaemonOperation.GetSelfWorkLocalJson();
        }

        [HttpPost]
        public void SaveSelfWorkLocalJson(WorkRequest request)
        {
            VirtualRoot.DaemonOperation.SaveSelfWorkLocalJson(request);
        }

        [HttpPost]
        public string GetGpuProfilesJson()
        {
            return VirtualRoot.DaemonOperation.GetGpuProfilesJson();
        }

        [HttpPost]
        public void SaveGpuProfilesJson()
        {
            string json = Request.Content.ReadAsStringAsync().Result;
            VirtualRoot.DaemonOperation.SaveGpuProfilesJson(json);
        }

        // 注意：已经通过url传参了，为了版本兼容性不能去掉[FromUri]了
        [HttpPost]
        public void SetAutoBootStart([FromUri] bool autoBoot, [FromUri] bool autoStart)
        {
            VirtualRoot.DaemonOperation.SetAutoBootStart(autoBoot, autoStart);
        }

        [HttpPost]
        public void StartOrCloseWs(bool isResetFailCount)
        {
            VirtualRoot.DaemonWsClient.OpenOrCloseWs(isResetFailCount);
        }

        [HttpPost]
        public ResponseBase RestartWindows([FromBody] object request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.RestartWindows();
        }

        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody] object request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.ShutdownWindows();
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody] WorkRequest request)
        {
            return VirtualRoot.DaemonOperation.StartMine(request);
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody] object request)
        {
            if (request == null)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.StopMine();
        }

        [HttpPost]
        public ResponseBase UpgradeLucky([FromBody] UpgradeLuckyRequest request)
        {
            return VirtualRoot.DaemonOperation.UpgradeLucky(request);
        }

        [HttpGet]
        [HttpPost]
        public List<OperationResultDto> GetOperationResults(long afterTime)
        {
            return VirtualRoot.OperationResultSet.Gets(afterTime);
        }

        [HttpGet]
        [HttpPost]
        public List<DriveDto> GetDrives()
        {
            return VirtualRoot.DriveSet.AsEnumerable().ToList();
        }

        [HttpPost]
        public ResponseBase SetVirtualMemory([FromBody] DataRequest<Dictionary<string, int>> request)
        {
            if (request == null || request.Data == null || request.Data.Count == 0)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.SetVirtualMemory(request.Data);
        }

        [HttpGet]
        [HttpPost]
        public List<LocalIpDto> GetLocalIps()
        {
            return VirtualRoot.LocalIpSet.AsEnumerable().ToList();
        }

        [HttpPost]
        public ResponseBase SetLocalIps(DataRequest<List<LocalIpInput>> request)
        {
            if (request == null || request.Data == null || request.Data.Count == 0)
            {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.SetLocalIps(request.Data);
        }
    }
}
