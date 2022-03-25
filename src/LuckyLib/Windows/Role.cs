using System.Security.Principal;

namespace Lucky.Windows {
    public static class Role {
        public static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
