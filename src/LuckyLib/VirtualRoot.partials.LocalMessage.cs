using Lucky.Core;
using Lucky.Hub;
using System;
using System.Collections.Generic;

namespace Lucky
{
    public enum OutEnum
    {
        None = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Success = 4
    }

    public interface IOut
    {
        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowError(string message, string header = "错误", int autoHideSeconds = 0, bool toConsole = false);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowInfo(string message, string header = "信息", int autoHideSeconds = 4, bool toConsole = false);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowWarn(string message, string header = "注意", int autoHideSeconds = 0, bool toConsole = false);

        /// <summary>
        /// autoHideSeconds 0表示不自动隐藏，即需要手动点击"知道了"按钮隐藏
        /// </summary>
        void ShowSuccess(string message, string header = "成功", int autoHideSeconds = 4, bool toConsole = false);
    }

    public class EmptyOut : IOut
    {
        public static EmptyOut Instance { get; private set; } = new EmptyOut();

        private EmptyOut() { }

        public void ShowError(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            // nothing need todo
        }

        public void ShowInfo(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            // nothing need todo
        }

        public void ShowSuccess(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            // nothing need todo
        }

        public void ShowWarn(string message, string header, int autoHideSeconds, bool toConsole = false)
        {
            // nothing need todo
        }
    }

    public class ConsoleOut : IOut
    {
        public ConsoleOut() { }

        public void ShowError(string message, string header = "错误", int autoHideSeconds = 0, bool toConsole = false)
        {
            LuckyConsole.UserError(message);
        }

        public void ShowInfo(string message, string header = "信息", int autoHideSeconds = 4, bool toConsole = false)
        {
            LuckyConsole.UserInfo(message);
        }

        public void ShowSuccess(string message, string header = "成功", int autoHideSeconds = 4, bool toConsole = false)
        {
            LuckyConsole.UserOk(message);
        }

        public void ShowWarn(string message, string header = "警告", int autoHideSeconds = 0, bool toConsole = false)
        {
            LuckyConsole.UserWarn(message);
        }
    }

    [MessageType(description: "添加本地消息")]
    public class AddLocalMessageCommand : Cmd
    {
        public AddLocalMessageCommand(ILocalMessage input)
        {
            this.Input = input;
        }

        public ILocalMessage Input { get; private set; }
    }

    public static partial class VirtualRoot
    {
        #region Out
        private static IOut _out;
        /// <summary>
        /// 输出到系统之外去
        /// </summary>
        public static IOut Out
        {
            set => _out = value;
            get => _out ?? EmptyOut.Instance;
        }
        #endregion

        #region LocalMessage
        public static void MyLocalInfo(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false)
        {
            LocalMessage(LocalMessageChannel.My, provider, LocalMessageType.Info, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void MyLocalWarn(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false)
        {
            LocalMessage(LocalMessageChannel.My, provider, LocalMessageType.Warn, content, outEnum: outEnum, toConsole: toConsole);
        }

        public static void MyLocalError(string provider, string content, OutEnum outEnum = OutEnum.None, bool toConsole = false)
        {
            LocalMessage(LocalMessageChannel.My, provider, LocalMessageType.Error, content, outEnum: outEnum, toConsole: toConsole);
        }

        private static readonly Dictionary<LocalMessageType, Action<string>> _consoleAction = new Dictionary<LocalMessageType, Action<string>>
        {
            [LocalMessageType.Info] = LuckyConsole.UserInfo,
            [LocalMessageType.Warn] = LuckyConsole.UserWarn,
            [LocalMessageType.Error] = LuckyConsole.UserError
        };
        public static void LocalMessage(LocalMessageChannel channel, string provider, LocalMessageType messageType, string consoleLine, string content, OutEnum outEnum)
        {
            if (_consoleAction.TryGetValue(messageType, out Action<string> action))
            {
                action(consoleLine);
            }
            LocalMessage(channel, provider, messageType, content, outEnum, toConsole: false);
        }

        public static void LocalMessage(LocalMessageChannel channel, string provider, LocalMessageType messageType, string content, OutEnum outEnum, bool toConsole)
        {
            switch (outEnum)
            {
                case OutEnum.None:
                    if (toConsole && _consoleAction.TryGetValue(messageType, out Action<string> action))
                    {
                        action(content);
                    }
                    break;
                case OutEnum.Info:
                    Out.ShowInfo(content, toConsole: toConsole);
                    break;
                case OutEnum.Warn:
                    Out.ShowWarn(content, autoHideSeconds: 4, toConsole: toConsole);
                    break;
                case OutEnum.Error:
                    Out.ShowError(content, autoHideSeconds: 4, toConsole: toConsole);
                    break;
                case OutEnum.Success:
                    Out.ShowSuccess(content, toConsole: toConsole);
                    break;
                default:
                    break;
            }
            Execute(new AddLocalMessageCommand(new LocalMessageData
            {
                Id = Guid.NewGuid(),
                Channel = channel.GetName(),
                Provider = provider,
                MessageType = messageType.GetName(),
                Content = content,
                Timestamp = DateTime.Now
            }));
        }
        #endregion
    }
}
