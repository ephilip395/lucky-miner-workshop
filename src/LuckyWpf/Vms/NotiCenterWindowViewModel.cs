using Lucky.Notifications;

namespace Lucky.Vms
{
    public class NotiCenterWindowViewModel : ViewModelBase, IOut
    {
        public static NotiCenterWindowViewModel Instance { get; private set; } = new NotiCenterWindowViewModel();

        private NotiCenterWindowViewModel() { }

        private INotificationMessageManager _manager;
        public INotificationMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new NotificationMessageManager();
                }
                return _manager;
            }
        }

        public void ShowError(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            if (toConsole)
            {
                LuckyConsole.UserError(message);
            }
            UIThread.Execute(() =>
            {
                NotificationMessageBuilder builder = NotificationMessageBuilder.CreateMessage(Manager);
                _ = builder.Error(header, message ?? string.Empty);
                _ = autoHideSeconds > 0
                    ? builder
                        .Dismiss()
                        .WithDelay(autoHideSeconds)
                        .Queue()
                    : builder
                        .Dismiss().WithButton("知道了", null)
                        .Queue();
            });
        }

        public void ShowWarn(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            if (toConsole)
            {
                LuckyConsole.UserWarn(message);
            }
            UIThread.Execute(() =>
            {
                NotificationMessageBuilder builder = NotificationMessageBuilder.CreateMessage(Manager);
                _ = builder.Warning(header, message ?? string.Empty);
                _ = autoHideSeconds > 0
                    ? builder
                        .Dismiss()
                        .WithDelay(autoHideSeconds)
                        .Queue()
                    : builder
                        .Dismiss().WithButton("知道了", null)
                        .Queue();
            });
        }

        public void ShowInfo(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            if (toConsole)
            {
                LuckyConsole.UserInfo(message);
            }
            UIThread.Execute(() =>
            {
                NotificationMessageBuilder builder = NotificationMessageBuilder.CreateMessage(Manager);
                _ = builder.Info(header, message ?? string.Empty);
                _ = autoHideSeconds > 0
                    ? builder
                    .Dismiss()
                    .WithDelay(autoHideSeconds)
                    .Queue()
                    : builder
                        .Dismiss().WithButton("知道了", null)
                        .Queue();
            });
        }

        public void ShowSuccess(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            if (toConsole)
            {
                LuckyConsole.UserOk(message);
            }
            UIThread.Execute(() =>
            {
                NotificationMessageBuilder builder = NotificationMessageBuilder.CreateMessage(Manager);
                _ = builder.Success(header, message);
                _ = autoHideSeconds > 0
                    ? builder
                    .Dismiss()
                    .WithDelay(autoHideSeconds)
                    .Queue()
                    : builder
                        .Dismiss().WithButton("知道了", null)
                        .Queue();
            });
        }
    }
}
