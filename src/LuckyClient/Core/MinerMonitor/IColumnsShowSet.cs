using System;
using System.Collections.Generic;

namespace Lucky.Core.MinerMonitor {
    public interface IColumnsShowSet {
        void AddOrUpdate(ColumnsShowData data);
        void Remove(Guid id);
        List<ColumnsShowData> GetAll();
    }
}
