using Microsoft.Win32;
using Lucky.Views;
using Lucky.Vms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Lucky
{
    public static class AppUtil
    {

        // dotnet 版本是否大于 4.5
        private static bool IsDotNetVersionGE45
        {
            get
            {
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
                using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    if (ndpKey != null)
                    {
                        object obj = ndpKey.GetValue("Release");
                        if (obj != null)
                        {
                            return (int)obj >= 378389;
                        }
                    }
                    return false;
                }
            }
        }

        // 这个机制在MinerTweak程序起作用但在MinerMonitor程序中会发生类型初始化错误不起作用，具体原因未知
        public static void Run<TApp>(bool withSplashWindow = false) where TApp : Application, IApp, new()
        {
            if (withSplashWindow)
            {
                SplashScreen splashScreen = new SplashScreen("splashwindow.png");
                splashScreen.Show(true);
            }
            if (IsDotNetVersionGE45)
            {
                TApp app = new TApp();
                _ = app.Run();
            }
            else
            {
                // 打开 dotnet 4.5 下载网页
                _ = Process.Start("https://luckycm.monsterorb.com/getDotNet.html");
            }

        }

        private static class SafeNativeMethods
        {

            // 异步显示窗口
            [DllImport(DllName.User32Dll)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);


            // 设置前景窗口
            [DllImport(DllName.User32Dll)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        // 因为每个App都配备有个NotiCenterWindow，所以热键的逻辑放在NotiCenterWindow中完成
        public static bool IsHotKeyEnabled { get; set; }

        private static Application _app;

        #region Init
        public static void Init(Application app)
        {

            _app = app;

            // 异常处理
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                if (e.ExceptionObject is Exception exception)
                {
                    Handle(exception);
                }
            };

            app.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
            {
                Handle(e.Exception);
                e.Handled = true;
            };

            UIThread.InitializeWithDispatcher(app.Dispatcher);

            // 渲染选项 = 软件渲染
            //if (ClientAppType.IsMinerTweak)
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            // 文化 = zh-CN
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");


            // ToolTip 显示时间
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(60000));


            // 应用退出
            app.Exit += (sender, e) =>
            {
                // 丢弃互斥锁
                _mutexApp?.Dispose();
            };

            // Session 结束
            app.SessionEnding += (sender, e) =>
            {
                SessionEndReasons reason;
                switch (e.ReasonSessionEnding)
                {
                    // 登出
                    case ReasonSessionEnding.Logoff:
                        reason = SessionEndReasons.Logoff;
                        break;
                    // 关闭
                    case ReasonSessionEnding.Shutdown:
                        reason = SessionEndReasons.SystemShutdown;
                        break;
                    default:
                        reason = SessionEndReasons.Logoff;
                        break;
                }

                // Session 结束事件处理
                VirtualRoot.SessionEndingEventHandler?.Invoke(sender, new SessionEndingEventArgs(reason)
                {
                    Cancel = e.Cancel
                });
            };
        }
        #endregion
        // 互斥锁
        private static Mutex _mutexApp;
        public static bool GetMutex(string name)
        {
            bool result;
            try
            {
                _mutexApp = new Mutex(true, name, out result);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private const int SW_SHOWNOMAL = 1;

        // 显示窗口
        public static void Show(Process instance)
        {
            _ = SafeNativeMethods.ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            _ = SafeNativeMethods.SetForegroundWindow(instance.MainWindowHandle);
        }


        public static T GetResource<T>(string key)
        {
            return _app == null ? (T)Application.Current.Resources[key] : (T)_app.Resources[key];
        }


        public static object GetResource(string key)
        {
            return _app == null ? Application.Current.Resources[key] : _app.Resources[key];
        }

        #region private methods
        // 处理异常，主要处理 ValidationExceptioin ，其他仅仅记录到调试日志
        private static void Handle(Exception e)
        {
            if (e == null)
            {
                return;
            }
            if (e is ValidationException)
            {
                DialogWindow.ShowSoftDialog(new DialogWindowViewModel(title: "验证失败", message: e.Message, icon: "Icon_Error"));
            }
            else
            {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion
    }
}
