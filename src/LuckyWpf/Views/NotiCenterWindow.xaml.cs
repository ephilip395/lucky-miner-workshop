using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Lucky.Views
{
    public partial class NotiCenterWindow : Window
    {
        public static NotiCenterWindow Instance { get; private set; } = null;

        public static void ShowWindow()
        {
            if (Instance == null)
            {
                Instance = new NotiCenterWindow();
            }
            Instance.Show();
        }

        #region 将通知窗口切到活动窗口上面去
        /// <summary>
        /// 将通知窗口切到活动窗口上面去
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerIsTopmost"></param>
        /// <param name="isNoOtherWindow">如果没有其它窗口就不需要响应窗口激活和非激活状态变更事件了</param>
        public static void Bind(Window owner, bool ownerIsTopmost = false)
        {
            if (Instance == null)
            {
                return;
            }
            if (ownerIsTopmost)
            {
                owner.Activated += TopMostOwner_Activated;
                owner.Deactivated += Owner_Deactivated;
            }
            else
            {
                owner.Activated += Owner_Activated;
            }
            owner.LocationChanged += OnLocationChanged;
        }

        private static void OnLocationChanged(object sender, EventArgs e)
        {
            if (Instance == null)
            {
                return;
            }
            //Window owner = (Window)sender;
            //Instance.Left = owner.Left + (owner.Width - Instance.Width) / 2;
            //Instance.Top = owner.WindowState == WindowState.Maximized ? 8 : owner.Top > 80 ? owner.Top - 80 : owner.Top + 4;
        }

        private static void Owner_Deactivated(object sender, EventArgs e)
        {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (owner.Topmost)
            {
                owner.Topmost = false;
            }
        }

        private static void Owner_Activated(object sender, EventArgs e)
        {
            Window owner = (Window)sender;
            OnLocationChanged(sender, e);
            SwitchOwner(owner);
        }

        private static void TopMostOwner_Activated(object sender, EventArgs e)
        {
            Window owner = (Window)sender;
            // 解决当主界面上方出现popup层时主窗口下面的控制台窗口可能会被windows绘制到上面的BUG
            if (!owner.Topmost && owner.WindowState == WindowState.Maximized)
            {
                owner.Topmost = true;
            }
            Owner_Activated(sender, e);
        }

        private static readonly HashSet<Window> _owners = new HashSet<Window>();

        private static void SwitchOwner(Window owner)
        {
            if (Instance == null)
            {
                return;
            }
            if (Instance.Owner != owner)
            {
                bool isOwnerIsTopMost = owner.Topmost;
                if (isOwnerIsTopMost)
                {
                    owner.Topmost = false;
                }
                if (owner.Owner == Instance)
                {
                    owner.Owner = Instance.Owner;
                }
                Instance.Owner = owner;
                if (isOwnerIsTopMost && owner.WindowState == WindowState.Maximized)
                {
                    owner.Topmost = true;
                    Instance.Topmost = true;
                }
            }
            if (_owners.Contains(owner))
            {
                if (owner.IsEnabled)
                {
                    _ = (Instance.Owner?.Activate());
                }
                else
                {
                    _ = Instance.Activate();
                }
            }
            else
            {
                owner.Closed += Owner_Closed;
                owner.IsVisibleChanged += Owner_IsVisibleChanged;
                owner.StateChanged += Owner_StateChanged;
                _ = _owners.Add(owner);
                _ = Instance.Activate();
            }
        }

        private static void Owner_Closed(object sender, EventArgs e)
        {
            _owners.Remove((Window)sender);
        }

        private static void Owner_StateChanged(object sender, EventArgs e)
        {
            if (Instance == null)
            {
                return;
            }
            Window owner = (Window)sender;
            if (Instance.Owner == owner && owner.WindowState == WindowState.Minimized)
            {
                Instance.Owner = null;
                if (!Instance.IsVisible)
                {
                    Instance.Show();
                }
            }
        }

        private static void Owner_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Instance == null)
            {
                return;
            }
            Window owner = (Window)sender;
            if (Instance.Owner == owner && !owner.IsVisible)
            {
                Instance.Owner = null;
            }
        }
        #endregion

        public NotiCenterWindowViewModel Vm => NotiCenterWindowViewModel.Instance;

        private IntPtr _thisWindowHandle;
        private HwndSource hwndSource;
        private NotiCenterWindow()
        {
            DataContext = Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            if (ClientAppType.IsMinerTweak)
            {
                Loaded += (sender, e) =>
                {
                    _thisWindowHandle = new WindowInteropHelper(this).Handle;
                    hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
                    hwndSource.AddHook(new HwndSourceHook(WndProc));
                    if (AppUtil.IsHotKeyEnabled)
                    {
                        HotKeyUtil.RegHotKey = (key) =>
                        {
                            if (!RegHotKey(key, out string message))
                            {
                                VirtualRoot.Out.ShowError(message, autoHideSeconds: 4);
                                return false;
                            }
                            else
                            {
                                VirtualRoot.Out.ShowSuccess($"热键Ctrl + Alt + {key.ToString()} 设置成功");
                                return true;
                            }
                        };
                    }
                };
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ClientAppType.IsMinerTweak)
            {
                if (AppUtil.IsHotKeyEnabled)
                {
                    SystemHotKey.UnRegHotKey(_thisWindowHandle, c_hotKeyId);
                }
                hwndSource?.Dispose();
                hwndSource = null;
            }
            base.OnClosed(e);
        }

        private bool RegHotKey(System.Windows.Forms.Keys key, out string message)
        {
            if (!SystemHotKey.RegHotKey(_thisWindowHandle, c_hotKeyId, SystemHotKey.KeyModifiers.Alt | SystemHotKey.KeyModifiers.Ctrl, key, out message))
            {
                message = $"Ctrl + Alt + {key} " + message;
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (ClientAppType.IsMinerTweak)
            {
                if (AppUtil.IsHotKeyEnabled)
                {
                    _ = Enum.TryParse(HotKeyUtil.GetHotKey(), out System.Windows.Forms.Keys hotKey);
                    if (!RegHotKey(hotKey, out string message))
                    {
                        VirtualRoot.Out.ShowWarn(message, header: "热键设置失败", toConsole: true);
                    }
                }
            }
        }

        private const int WM_HOTKEY = 0x312;
        private const int c_hotKeyId = 1; //热键ID（自定义）
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    int tmpWParam = wParam.ToInt32();
                    if (tmpWParam == c_hotKeyId)
                    {
                        VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: true));
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Normal)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
