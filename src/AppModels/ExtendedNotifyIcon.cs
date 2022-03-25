using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Lucky
{
    public class ExtendedNotifyIcon
    {
        public static ExtendedNotifyIcon Create(string text, bool isMinerMonitor)
        {
            string url;
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            if (mainAssembly == null)
            {
                throw new InvalidProgramException();
            }
            var name = mainAssembly.GetName().Name;
            url = $"pack://application:,,,/{name};component/logo.ico";
            Icon icon = new Icon(System.Windows.Application.GetResourceStream(new Uri(url)).Stream);
            return new ExtendedNotifyIcon(icon, text, isMinerMonitor);
        }

        private readonly NotifyIcon _targetNotifyIcon;
        private readonly bool _isMinerMonitor;
        private ExtendedNotifyIcon(Icon icon, string text, bool isMinerMonitor)
        {
            _isMinerMonitor = isMinerMonitor;
            _targetNotifyIcon = new NotifyIcon
            {
                Icon = icon,
                Visible = isMinerMonitor || LuckyContext.Instance.MinerProfile.IsShowNotifyIcon,
                Text = text,
                ContextMenuStrip = new ContextMenuStrip
                {
                    BackColor = Color.White,
                    ShowImageMargin = false
                }
            };
            _targetNotifyIcon.ContextMenuStrip.Items.Add("退出" + text, null, (sender, e) =>
            {
                VirtualRoot.Execute(new CloseLuckyCommand("手动操作"));
            });
            _targetNotifyIcon.MouseDown += (object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: true));
                }
            };
            VirtualRoot.BuildEventPath<AppExitEvent>("退出托盘图标", LogEnum.None, typeof(ExtendedNotifyIcon), PathPriority.Normal, path: message =>
            {
                _targetNotifyIcon.Dispose();
            });
        }

        public void RefreshIcon()
        {
            _targetNotifyIcon.Visible = _isMinerMonitor || LuckyContext.Instance.MinerProfile.IsShowNotifyIcon;
        }
    }
}
