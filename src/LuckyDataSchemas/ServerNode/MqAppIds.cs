using System.Collections.Generic;

namespace Lucky.ServerNode {
    public class MqAppIds {
        public MqAppIds() {
            this.AppIds = new List<string>();
        }

        public List<string> AppIds { get; set; }

        /// <summary>
        /// 带出这个节点的数据
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 带的节点的数据
        /// </summary>
        public MqCountData MqCountData { get; set; }
    }
}
