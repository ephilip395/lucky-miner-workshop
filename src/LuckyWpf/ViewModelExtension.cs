using Lucky.Views;
using Lucky.Vms;

namespace Lucky {
    public static class ViewModelExtension {
        // 起编译时免导入DialogWindow所在的Lucky.Views命名空间的作用
        public static void ShowSoftDialog(this ViewModelBase _, DialogWindowViewModel config) {
            DialogWindow.ShowSoftDialog(config);
        }
    }
}
