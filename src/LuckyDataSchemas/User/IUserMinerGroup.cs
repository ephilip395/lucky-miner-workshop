using Lucky.Core;

namespace Lucky.User {
    public interface IUserMinerGroup : IMinerGroup {
        string LoginName { get; }
    }
}
