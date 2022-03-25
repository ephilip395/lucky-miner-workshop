﻿using Lucky.Hub;
using Lucky.Views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Lucky
{
    public static class WindowExtension
    {
        /// <summary>
        /// 基于鼠标位置放置窗口
        /// </summary>
        /// <param name="window"></param>
        public static void MousePosition(this Window window)
        {
            if (window.Owner == null)
            {
                return;
            }
            if (SafeNativeMethods.GetCursorPos(out POINT pt))
            {
                var width = window.Width.Equals(double.NaN) ? 400 : window.Width;
                var height = window.Height.Equals(double.NaN) ? 200 : window.Height;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                double left = pt.X - width / 2;
                double top = pt.Y + 20;
                if (left < window.Owner.Left)
                {
                    left = window.Owner.Left;
                }
                var ownerTop = window.Owner.Top;
                var ownerLeft = window.Owner.Left;
                if (window.Owner.WindowState == WindowState.Maximized)
                {
                    ownerTop = 0;
                    ownerLeft = 0;
                }
                var over = top + height - ownerTop - window.Owner.Height;
                if (over > 0)
                {
                    top = pt.Y - height - 20;
                }
                over = left + width - ownerLeft - window.Owner.Width;
                if (over > 0)
                {
                    left -= over;
                }
                if (top > SystemParameters.WorkArea.Height)
                {
                    top = SystemParameters.WorkArea.Height - height;
                }
                if (left > SystemParameters.WorkArea.Width)
                {
                    left = SystemParameters.WorkArea.Width - width;
                }
                window.Left = left;
                window.Top = top;
            }
        }

        /// <summary>
        /// 打开为软对话框，非模态对话框。效果是打开时遮罩和禁用父窗口，关闭时复原父窗口。
        /// </summary>
        /// <param name="window"></param>
        public static void ShowSoftDialog(this Window window)
        {
            ShowDialog(window, useSoftDialog: true);
        }

        public static void ShowHardDialog(this Window window)
        {
            ShowDialog(window, useSoftDialog: false);
        }

        private static void ShowDialog(Window window, bool useSoftDialog)
        {
            if (window.Owner == null)
            {
                var owner = WpfUtil.GetTopWindow();
                if (owner != null && owner != window && owner.GetType() != typeof(NotiCenterWindow))
                {
                    window.Owner = owner;
                }
            }
            if (window.Owner != null)
            {

                double ownerOpacity = window.Owner.Opacity;
                window.Owner.Opacity = 0.6;
                window.Owner.IsEnabled = false;
                window.Closing += (sender, e) =>
                {
                    window.Owner.Opacity = ownerOpacity;
                    window.Owner.IsEnabled = true;
                };
                window.Closed += (sender, e) =>
                {
                    window.Owner.Activate();
                };
            }
            if (useSoftDialog)
            {
                window.Show();
            }
            else
            {
                window.ShowDialog();
            }
        }

        public static void ShowWindow(this Window window, bool isToggle)
        {
            if (isToggle)
            {
                if (window.IsVisible && window.WindowState != WindowState.Minimized)
                {
                    window.Hide();
                }
                else
                {
                    window.Show();
                    if (window.WindowState == WindowState.Minimized)
                    {
                        window.WindowState = WindowState.Normal;
                    }
                }
            }
            else
            {
                window.Show();
                if (window.WindowState == WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                }
            }
        }

        private const string messagePathIdsResourceKey = "messagePathIds";

        public static void BuildCmdPath<TCmd>(this Window window, LogEnum logType, Type location, Action<TCmd> path)
            where TCmd : ICmd
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (window.Resources == null)
            {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null)
            {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed;
            }
            MessageTypeAttribute messageTypeDescription = MessageTypeAttribute.GetMessageTypeAttribute(typeof(TCmd));
            var messagePathId = VirtualRoot.BuildCmdPath(location, logType, path);
            messagePathIds.Add(messagePathId);
        }

        public static void BuildEventPath<TEvent>(this Window window, string description, LogEnum logType, Type location, PathPriority priority, Action<TEvent> path)
            where TEvent : IEvent
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (window.Resources == null)
            {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null)
            {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed; ;
            }
            var messagePathId = VirtualRoot.BuildMessagePath(description, logType, location, priority, path);
            messagePathIds.Add(messagePathId);
        }

        private static void BuildOnecePath<TMessage>(this Window window, string description, LogEnum logType, Guid pathId, Type location, PathPriority priority, Action<TMessage> path)
            where TMessage : IMessage
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (window.Resources == null)
            {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null)
            {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed; ;
            }
            var messagePathId = VirtualRoot.BuildOncePath(description, logType, pathId, location, priority, path);
            messagePathIds.Add(messagePathId);
        }

        public static void BuildCloseWindowOncePath(this Window window, Guid pathId)
        {
            window.BuildOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, pathId: pathId, location: typeof(WindowExtension), PathPriority.Normal, path: message =>
            {
                UIThread.Execute(() => window.Close());
            });
        }

        public static IMessagePathId BuildViaTimesLimitPath<TMessage>(this Window window, string description, LogEnum logType, int viaTimesLimit, Type location, PathPriority priority, Action<TMessage> path)
            where TMessage : IMessage
        {
            if (WpfUtil.IsInDesignMode)
            {
                return null;
            }
            if (window.Resources == null)
            {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null)
            {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed; ;
            }
            var messagePathId = VirtualRoot.BuildViaTimesLimitPath(description, logType, viaTimesLimit, location, priority, path);
            messagePathIds.Add(messagePathId);
            return messagePathId;
        }

        private static void UiElement_Closed(object sender, EventArgs e)
        {
            FrameworkElement uiElement = (FrameworkElement)sender;
            List<IMessagePathId> pathIds = (List<IMessagePathId>)uiElement.Resources[messagePathIdsResourceKey];
            foreach (var pathId in pathIds)
            {
                VirtualRoot.RemoveMessagePath(pathId);
            }
        }
    }
}
