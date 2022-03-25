using System.ComponentModel;

namespace Lucky.Core.Kernels {
    public enum InstallStatus {
        [Description("未安装")]
        Uninstalled = 0,
        [Description("已安装")]
        Installed = 1
    }
}
