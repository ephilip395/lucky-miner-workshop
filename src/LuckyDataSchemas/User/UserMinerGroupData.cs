using Lucky.Core;

namespace Lucky.User {
    public class UserMinerGroupData : MinerGroupData, IUserMinerGroup {
        public UserMinerGroupData() { }

        public string LoginName { get; set; }
    }
}
