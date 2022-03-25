namespace Lucky.Core.MinerServer {
    public static class MinerDataExtensions {
        public static string GetMinerText(this IMinerData minerData) {
            return $"{minerData.MinerName}({minerData.MinerIp})";
        }
    }
}
