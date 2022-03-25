using Lucky.Controllers;
using System;
using System.Web.Http;

namespace Lucky {
    /// <summary>
    /// 端口号：<see cref="LuckyKeyword.MinerMonitorPort"/>
    /// </summary>
    public class MinerMonitorController : ApiController, IMinerMonitorController {
        [HttpPost]
        public bool ShowMainWindow() {
            try {
                VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: false));
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        [HttpPost]
        public ResponseBase CloseMinerMonitor([FromBody]object request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                100.MillisecondsDelay().ContinueWith(t => {
                    VirtualRoot.Execute(new CloseLuckyCommand("群控端升级成功后关闭旧版客户端"));
                });
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }
    }
}
