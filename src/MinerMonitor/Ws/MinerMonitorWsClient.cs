using System;

namespace Lucky.Ws
{
    public class MinerMonitorWsClient : AbstractWsClient
    {
        public MinerMonitorWsClient() : base(LuckyAppType.MinerMonitor)
        {
        }

        protected override bool TryGetHandler(string messageType, out Action<Action<WsMessage>, WsMessage> handler)
        {
            return MinerMonitorWsMessageFromWsServerHandler.TryGetHandler(messageType, out handler);
        }

        protected override void UpdateWsStateAsync(string description, bool toOut)
        {
            var state = base.GetState();
            if (!string.IsNullOrEmpty(description))
            {
                state.Description = description;
            }
            state.ToOut = toOut;
            VirtualRoot.Execute(new RefreshWsStateCommand(state));
        }
    }
}
