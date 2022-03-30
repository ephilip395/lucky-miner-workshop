using System;

namespace Lucky.Ws
{
    // 考虑设计个自定义数据协议，用二进制传输，除了Data之外的字段放在Head部分且加入一个HeadLength字段，Head部分的第一个int字节是HeadLength，用RLP编码头部。
    // 但是二进制对将来的web群控端不友好，javascript不擅长处理二进制；
    // 这么滴吧，经测试序列化和反序列化挺快的，不会造成瓶颈，那就先传字符串序列化和反序列化去吧，将来如有必要再开辟新路径制定二进制协议传二进制；
    public class WsMessage : IData
    {
        #region message type 注意这些常量不区分大小写，注意唯一性
        /// <summary>
        /// 服务端是个集群，两种情况下会向挖矿端发送ReGetServerAddress指令：
        /// 1. 当集群中上线了新节点后，可以计算出哪些客户端会分配到新节点去，从而通知这些节点重新获取服务器地址连向新节点；
        /// 2. 每个服务器节点周期（比如1分钟）检查自己持有的每个客户端连接是否还是分片到自己的客户端，如果不是则发送该消息要求客户端重新获取服务器地址。
        /// 以上两种情况中其中第二种是防护措施通常不会行走到。
        /// </summary>
        public const string ReGetServerAddress = "ReGetServerAddress";          // WsServer->MinerTweak || WsServer->MinerMonitor
        public const string UpdateAESPassword = "UpdateAESPassword";            // WsServer-MinerTweak
        public const string ServerTime = "ServerTime";                          // WsServer->MinerMonitor
        /// <summary>
        /// 指的是用户的密码变更后群控端需要重新登录而不是指挖矿端，挖矿端有连接概念并没有登录概念。
        /// </summary>
        public const string ReLogin = "ReLogin";                                // WebApiServer:UserPasswordChangedMqMessage->Mq->WsServer->MinerMonitor
        public const string GetConsoleOutLines = "GetConsoleOutLines";          // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string FastGetConsoleOutLines = "FastGetConsoleOutLines";  // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetLocalMessages = "GetLocalMessages";              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string FastGetLocalMessages = "FastGetLocalMessages";      // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetOperationResults = "GetOperationResults";        // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string FastGetOperationResults = "FastGetOperationResults";// MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetDrives = "GetDrives";                            // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetLocalIps = "GetLocalIps";                        // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetSpeed = "GetSpeed";                              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        /// <summary>
        /// SelfWorkLocalJson是存储在矿机上的
        /// </summary>
        public const string GetSelfWorkLocalJson = "GetSelfWorkLocalJson";      // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string GetGpuProfilesJson = "GetGpuProfilesJson";          // MinerMonitor->WsServer->Mq->WsServer->MinerTweak

        public const string EnableRemoteDesktop = "EnableRemoteDesktop";        // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string BlockWAU = "BlockWAU";                              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string SwitchRadeonGpu = "SwitchRadeonGpu";                // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string SetVirtualMemory = "SetVirtualMemory";              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string SetLocalIps = "SetLocalIps";                        // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        /// <summary>
        /// SelfWorkLocalJson是存储在矿机上的
        /// </summary>
        public const string SaveSelfWorkLocalJson = "SaveSelfWorkLocalJson";    // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string SaveGpuProfilesJson = "SaveGpuProfilesJson";        // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string SetAutoBootStart = "SetAutoBootStart";              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string UpdateConnParams = "UpdateConnParams";
        public const string RestartWindows = "RestartWindows";                  // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string ShutdownWindows = "ShutdownWindows";                // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        public const string UpgradeLucky = "UpgradeLucky";                  // MinerMonitor->WsServer->Mq->WsServer->MinerTweak

        // WebApiServer和WsServer都会关心这类消息，WebApiServer只关心作业消息，WsServer只关心非作业消息。
        // WebApiServer->StartWorkMineMq->WsServer->MinerTweak
        public const string StartMine = "StartMine";                            // MinerMonitor->WsServer->Mq->WsServer->MinerTweak
        // WebApiServer关心StartMine类消息，如果WebApiServer发现StartMine消息是作业消息则会重整成StartWorkMine消息
        public const string StartWorkMine = "StartWorkMine";                    // WebApiServer->Mq->WsServer->MinerTweak
        public const string StopMine = "StopMine";                              // MinerMonitor->WsServer->Mq->WsServer->MinerTweak

        public const string QueryClientDatas = "QueryClientDatas";              // MinerMonitor->WsServer->Mq->WebApiServer
        public const string AutoQueryClientDatas = "AutoQueryClientDatas";      // MinerMonitor->WsServer->Mq->WebApiServer

        public const string ConsoleOutLines = "ConsoleOutLines";                // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string LocalMessages = "LocalMessages";                    // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string OperationResults = "OperationResults";              // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string Drives = "Drives";                                  // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string LocalIps = "LocalIps";                              // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string Speed = "Speed";                                    // MinerTweak->WsServer->Mq->WebApiServer
        public const string OperationReceived = "OperationReceived";            // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string SelfWorkLocalJson = "SelfWorkLocalJson";            // MinerTweak->WsServer->Mq->WsServer->MinerMonitor
        public const string GpuProfilesJson = "GpuProfilesJson";                // MinerTweak->WsServer->Mq->WsServer->MinerMonitor

        public const string CalcConfigs = "CalcConfigs";                        // MinerTweak->WsServer->MinerTweak
        public const string QueryCalcConfigs = "QueryCalcConfigs";              // MinerTweak->WsServer->MinerTweak

        /// <summary>
        /// WsServer收到来自MinerMonitor的 <see cref="QueryClientDatas"/> WsMessage时往会Mq发送QueryClientsForWsRoutingKey消息，
        /// WebApiServer订阅了QueryClientsForWsRoutingKey消息并将QueryClientsForWsResponseMqMessage消息发送回到Mq，WsServer订阅了
        /// QueryClientsForWsResponseMqMessage消息将本ClientDatas消息发送给MinerMonitor。
        /// </summary>
        public const string ClientDatas = "ClientDatas";                        // WsServer->MinerMonitor
        #endregion

        public WsMessage()
        {
        }

        public WsMessage(Guid id, string type)
        {
            this.Id = id;
            this.Type = type;
            this.Timestamp = Lucky.Timestamp.GetTimestamp();
        }

        public Guid Id { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public string Sign { get; set; }

        public object Data { get; set; }
    }
}
