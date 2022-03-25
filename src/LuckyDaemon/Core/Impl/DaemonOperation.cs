﻿using Lucky.Controllers;
using Lucky.Core.Daemon;
using Lucky.Core.MinerTweak;
using Lucky.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Lucky.Core.Impl
{
    public class DaemonOperation : IDaemonOperation
    {
        private static readonly string _minerClientControllerName = ControllerUtil.GetControllerName<IMinerTweakController>();

        public DaemonOperation() { }

        #region EnableRemoteDesktop
        public ResponseBase EnableRemoteDesktop()
        {
            ResponseBase response;
            try
            {
                LuckyRegistry.SetIsRdpEnabled(true);
                Firewall.AddRdpRule();
                if (IsLuckyOpened())
                {
                    RpcRoot.JsonRpc.FirePostAsync(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.RefreshIsRemoteDesktopEnabled), null, data: null, timeountMilliseconds: 3000);
                }
                response = ResponseBase.Ok("开启Windows远程桌面");
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        private ResponseBase RunAction(MinerTweakActionType actionType)
        {
            if (IsLuckyOpened())
            {
                var request = new DataRequest<MinerTweakActionType>
                {
                    Data = actionType
                };
                RpcRoot.JsonRpc.FirePostAsync(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.RunAction), null, data: request);
            }
            else if (TryGetMinerTweakLocation(out string location))
            {
                Windows.Cmd.RunClose(location, $"{LuckyKeyword.ActionCmdParameterName}{actionType.ToString()}");
            }
            ResponseBase response = ResponseBase.Ok(actionType.GetDescription());
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        #region SwitchRadeonGpu
        public ResponseBase SwitchRadeonGpu(bool on)
        {
            return RunAction(on ? MinerTweakActionType.SwitchRadeonGpuOn : MinerTweakActionType.SwitchRadeonGpuOff);
        }
        #endregion

        #region BlockWAU
        public ResponseBase BlockWAU()
        {
            return RunAction(MinerTweakActionType.BlockWAU);
        }
        #endregion

        #region CloseDaemon
        public void CloseDaemon()
        {
            // 延迟100毫秒再退出从而避免当前的CloseDaemon请求尚未收到响应
            100.MillisecondsDelay().ContinueWith(t =>
            {
                VirtualRoot.Exit();
            });
        }
        #endregion

        #region GetSelfWorkLocalJson
        public string GetSelfWorkLocalJson()
        {
            try
            {
                return SpecialPath.ReadSelfWorkLocalJsonFile();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
        #endregion

        #region SaveSelfWorkLocalJson
        public bool SaveSelfWorkLocalJson(WorkRequest request)
        {
            bool isSuccess = false;
            string description = "保存单机作业";
            if (request == null)
            {
                isSuccess = false;
                description = "参数错误";
            }
            else
            {
                try
                {
                    SpecialPath.WriteSelfWorkLocalJsonFile(request.LocalJson);
                    SpecialPath.WriteSelfWorkServerJsonFile(request.ServerJson);
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                }
            }
            VirtualRoot.OperationResultSet.Add(new OperationResultData
            {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = isSuccess ? 200 : 500,
                ReasonPhrase = isSuccess ? "Ok" : "Fail",
                Description = description
            });
            return isSuccess;
        }
        #endregion

        #region GetGpuProfilesJson
        public string GetGpuProfilesJson()
        {
            try
            {
                return SpecialPath.ReadGpuProfilesJsonFile();
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
        #endregion

        #region SaveGpuProfilesJson
        public bool SaveGpuProfilesJson(string json)
        {
            bool isSuccess = false;
            string description = "超频";
            try
            {
                SpecialPath.SaveGpuProfilesJsonFile(json);
                if (IsLuckyOpened())
                {
                    RpcRoot.JsonRpc.FirePostAsync(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.OverClock), null, data: null);
                }
                else
                {
                    description = "超频，挖矿端未启动，下次启动时生效";
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.OperationResultSet.Add(new OperationResultData
            {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = isSuccess ? 200 : 500,
                ReasonPhrase = isSuccess ? "Ok" : "Fail",
                Description = description
            });
            return isSuccess;
        }
        #endregion

        #region SetAutoBootStart
        public bool SetAutoBootStart(bool autoBoot, bool autoStart)
        {
            bool isSuccess = false;
            try
            {
                MinerProfileUtil.SetAutoStart(autoBoot, autoStart);
                if (IsLuckyOpened())
                {
                    RpcRoot.JsonRpc.FirePostAsync(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.RefreshAutoBootStart), null, data: null, timeountMilliseconds: 3000);
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.OperationResultSet.Add(new OperationResultData
            {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = isSuccess ? 200 : 500,
                ReasonPhrase = isSuccess ? "Ok" : "Fail",
                Description = "设置"
            });
            return isSuccess;
        }
        #endregion

        #region RestartWindows
        public ResponseBase RestartWindows()
        {
            ResponseBase response;
            try
            {
                Windows.Power.Restart(10);
                CloseLucky();
                response = ResponseBase.Ok("重启矿机");
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region ShutdownWindows
        public ResponseBase ShutdownWindows()
        {
            ResponseBase response;
            try
            {
                Windows.Power.Shutdown(10);
                CloseLucky();
                response = ResponseBase.Ok("关闭矿机");
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region IsLuckyOpened
        public bool IsLuckyOpened()
        {
            if (TryGetMinerTweakLocation(out string location))
            {
                string processName = Path.GetFileNameWithoutExtension(location);
                Process[] processes = Process.GetProcessesByName(processName);
                return processes.Length != 0;
            }
            return false;
        }
        #endregion

        #region StartMine
        public ResponseBase StartMine(WorkRequest request)
        {
            ResponseBase response;
            if (request == null)
            {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else
            {
                try
                {
                    // 单机作业的localJson和serverJson是在编辑单机作业时提前传递到挖矿端的所以不需要在开始挖矿时再传递
                    if (request.WorkId != Guid.Empty && !request.WorkId.IsSelfMineWorkId())
                    {
                        SpecialPath.WriteMineWorkLocalJsonFile(request.LocalJson);
                        SpecialPath.WriteMineWorkServerJsonFile(request.ServerJson);
                    }
                    if (IsLuckyOpened())
                    {
                        WorkRequest innerRequest = new WorkRequest
                        {
                            WorkId = request.WorkId,
                            WorkerName = request.WorkerName
                        };
                        RpcRoot.JsonRpc.PostAsync<ResponseBase>(
                            LuckyKeyword.Localhost,
                            LuckyKeyword.MinerTweakPort,
                            _minerClientControllerName,
                            nameof(IMinerTweakController.StartMine),
                            innerRequest,
                            callback: null,
                            timeountMilliseconds: 3000);
                        response = ResponseBase.Ok("开始挖矿");
                    }
                    else
                    {
                        string location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
                        if (!string.IsNullOrEmpty(location) && File.Exists(location))
                        {
                            string arguments = LuckyKeyword.AutoStartCmdParameterName;
                            if (request.WorkId != Guid.Empty)
                            {
                                if (request.WorkId.IsSelfMineWorkId())
                                {
                                    arguments = "--selfWork " + arguments;
                                }
                                else
                                {
                                    arguments = "--work " + arguments;
                                }
                            }
                            Windows.Cmd.RunClose(location, arguments);
                            response = ResponseBase.Ok("开始挖矿");
                        }
                        else
                        {
                            response = ResponseBase.ServerError("开始挖矿，未找到挖矿端程序");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region StopMine
        public ResponseBase StopMine()
        {
            ResponseBase response;
            try
            {
                if (!IsLuckyOpened())
                {
                    response = ResponseBase.Ok();
                }
                else
                {
                    RpcRoot.JsonRpc.PostAsync<ResponseBase>(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.StopMine), new object(), callback: null, timeountMilliseconds: 3000);
                    response = ResponseBase.Ok("停止挖矿");
                }
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region UpgradeLucky
        public ResponseBase UpgradeLucky(UpgradeLuckyRequest request)
        {
            ResponseBase response;
            if (request == null || string.IsNullOrEmpty(request.LuckyFileName))
            {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
                        if (!string.IsNullOrEmpty(location) && File.Exists(location))
                        {
                            string arguments = LuckyKeyword.UpgradeCmdParameterName + request.LuckyFileName;
                            Windows.Cmd.RunClose(location, arguments);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorDebugLine(e);
                    }
                });
                response = ResponseBase.Ok("升级挖矿端");
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region SetVirtualMemory
        public ResponseBase SetVirtualMemory(Dictionary<string, int> data)
        {
            ResponseBase response;
            if (data == null || data.Count == 0)
            {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else
            {
                try
                {
                    VirtualRoot.DriveSet.SetVirtualMemory(data);
                    response = ResponseBase.Ok("设置虚拟内存，需重启电脑，重启电脑后生效");
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region SetLocalIps
        public ResponseBase SetLocalIps(List<LocalIpInput> data)
        {
            ResponseBase response;
            if (data == null || data.Count == 0)
            {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else
            {
                try
                {
                    foreach (var localIpInput in data)
                    {
                        VirtualRoot.Execute(new SetLocalIpCommand(localIpInput, localIpInput.IsAutoDNSServer));
                    }
                    response = ResponseBase.Ok("设置矿机IP，可能会掉线，通常过一会儿能恢复。");
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
        #endregion

        #region private static methods
        private static bool TryGetMinerTweakLocation(out string location)
        {
            location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
            return !string.IsNullOrEmpty(location) && File.Exists(location);
        }

        private static void CloseLucky()
        {
            RpcRoot.JsonRpc.PostAsync<ResponseBase>(LuckyKeyword.Localhost, LuckyKeyword.MinerTweakPort, _minerClientControllerName, nameof(IMinerTweakController.CloseLucky), new object { }, (response, e) =>
            {
                bool isClosed = response.IsSuccess();
                if (!isClosed)
                {
                    try
                    {
                        string location = LuckyRegistry.GetLocation(LuckyAppType.MinerTweak);
                        if (!string.IsNullOrEmpty(location) && File.Exists(location))
                        {
                            string processName = Path.GetFileNameWithoutExtension(location);
                            Windows.TaskKill.Kill(processName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorDebugLine(ex);
                    }
                }
            }, timeountMilliseconds: 3000);
        }
        #endregion
    }
}
