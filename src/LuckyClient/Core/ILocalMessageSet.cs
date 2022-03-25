using Lucky.Core.MinerTweak;
using System.Collections.Generic;

namespace Lucky.Core
{
    public interface ILocalMessageSet
    {
        ILocalMessageDtoSet LocalMessageDtoSet { get; }
        IEnumerable<ILocalMessage> AsEnumerable();
    }
}
