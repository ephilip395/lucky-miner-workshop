﻿using System.Text;

namespace Lucky.Gpus {
    public class QueryGpuNameCountsRequest : IPagingRequest, ISignableData {
        public QueryGpuNameCountsRequest() { }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("54CD4572-760B-4480-AAED-F5E5DCFE3C1F");
        }
    }
}
