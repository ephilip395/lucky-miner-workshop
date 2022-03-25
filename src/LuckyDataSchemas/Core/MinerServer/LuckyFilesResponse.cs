using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerServer {
    public class LuckyFilesResponse : ResponseBase {
        public LuckyFilesResponse() {
            this.Data = new List<LuckyFileData>();
        }

        public static LuckyFilesResponse Ok(List<LuckyFileData> data, DateTime timestamp) {
            return new LuckyFilesResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Timestamp = timestamp
            };
        }

        public List<LuckyFileData> Data { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
