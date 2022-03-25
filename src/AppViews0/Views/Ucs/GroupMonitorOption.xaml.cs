using Lucky.Vms;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lucky.Views.Ucs
{
    public partial class GroupMonitorOption : UserControl
    {
        public MinerProfileViewModel Vm { get; private set; }

        private readonly Brush _outerUserGroupBg;

        public GroupMonitorOption()
        {
            this.Vm = AppRoot.MinerProfileVm;
            this.DataContext = AppRoot.MinerProfileVm;
            InitializeComponent();
            _outerUserGroupBg = OuterUserGroup.BorderBrush;
            this.OnLoaded(window =>
            {
                VirtualRoot.BuildEventPath<SignUpedEvent>(
                    "注册了新外网群控用户后自动填入外网群控用户名",
                    LogEnum.None, this.GetType(), PathPriority.Normal, path: message =>
                    {
                        this.Vm.OuterUserId = message.LoginName;
                    });
            });
        }

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z)
            {
                Vm.HotKey = e.Key.ToString();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        // 高亮外网群控区域
        public void HighlightOuterUser()
        {
            OuterUserGroup.BorderBrush = WpfUtil.RedBrush;
            OuterUserGroup.BringIntoView();
            TimeSpan.FromSeconds(1).Delay().ContinueWith(t =>
            {
                UIThread.Execute(() =>
                {
                    OuterUserGroup.BorderBrush = _outerUserGroupBg;
                });
            });
        }


        private void TbWsDaemonStateDescription_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Vm.RefreshWsDaemonState();
        }
    }
}
