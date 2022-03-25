﻿using System;
using System.Linq;

namespace Lucky.User {
    public static class LoginedUserExtensions {
        public static bool IsAdmin(this ILoginedUser user) {
            if (user == null || string.IsNullOrEmpty(user.Roles)) {
                return false;
            }
            return user.Roles.Split(',').Contains(nameof(Role.RoleEnum.Admin), StringComparer.OrdinalIgnoreCase);
        }
    }
}
