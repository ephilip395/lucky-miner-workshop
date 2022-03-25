using System.Collections.Generic;

namespace Lucky.Core
{
    public interface ILocalMessageDtoSet
    {
        void Add(LocalMessageDto data);
        List<LocalMessageDto> Gets(long afterTime);
    }
}
