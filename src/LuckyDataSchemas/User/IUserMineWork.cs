using Lucky.Core;

namespace Lucky.User {
    public interface IUserMineWork : IMineWork {
        string LoginName { get; }
    }
}
