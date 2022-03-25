using System.Collections.Generic;

namespace Lucky.Core {
    public interface IOperationResultSet {
        void Add(OperationResultDto operationResult);
        List<OperationResultDto> Gets(long afterTime);
    }
}
