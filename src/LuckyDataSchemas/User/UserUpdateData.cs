using System.Text;

namespace Lucky.User {
    public class UserUpdateData : ISignableData {
        public UserUpdateData() {
        }

        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
