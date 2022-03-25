namespace Lucky.User {
    public interface IUserAppSetting : IAppSetting {
        string Id { get; }
        string LoginName { get; }
    }
}
