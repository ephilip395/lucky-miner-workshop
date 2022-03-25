using Lucky.Core;

namespace Lucky.User {
    public class UserMineWorkData : MineWorkData, IUserMineWork {
        public UserMineWorkData() {
        }

        public string LoginName { get; set; }
    }
}
