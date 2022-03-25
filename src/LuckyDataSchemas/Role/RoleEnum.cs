using System.ComponentModel;

namespace Lucky.Role {
    public enum RoleEnum {
        [Description("超管")]
        Admin,
        [Description("普通用户")]
        User
    }
}
