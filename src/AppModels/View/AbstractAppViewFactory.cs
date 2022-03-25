using System;
using System.Diagnostics;
using System.Windows;

namespace Lucky.View {
    public abstract class AbstractAppViewFactory : IAppViewFactory {

        private static readonly object _locker = new object();
        private static Window _mainWindow = null;

        public abstract void BuildPaths();
        public abstract Window CreateMainWindow();

        public AbstractAppViewFactory() {

            // 支持退出应用
            VirtualRoot.BuildCmdPath<CloseLuckyCommand>(
                location: typeof(AbstractAppViewFactory),
                LogEnum.DevConsole,
                path: message => {

                // 不能推迟这个日志记录的时机，因为推迟会有 windows 异常日志
                VirtualRoot.MyLocalInfo(nameof(AbstractAppViewFactory), $"退出{VirtualRoot.AppName}。原因：{message.Reason}");
                UIThread.Execute(() => {
                    try {
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex) {
                        Logger.ErrorDebugLine(ex);
                        Environment.Exit(0);
                    }
                });

            });
        }

        // 显示主窗口
        public void ShowMainWindow(bool isToggle, out Window mainWindow) {
            if (_mainWindow == null) {
                lock (_locker) {
                    if (_mainWindow == null) {
                        _mainWindow = CreateMainWindow();
                        LuckyContext.RefreshArgsAssembly.Invoke("主界面创建后");
                        _mainWindow.Show();
                    }
                }
            }
            else {
                AppRoot.Enable();
                bool needActive = _mainWindow.WindowState != WindowState.Minimized;
                _mainWindow.ShowWindow(isToggle);
                if (needActive) {
                    _mainWindow.Activate();
                }
            }
            mainWindow = _mainWindow;
        }


        // 显示主窗口
        // 这个方法看不明白为什么显示主窗口需要执行 app.Shutdown()
        public void ShowMainWindow(Application app) {
            try {
                switch (ClientAppType.AppType) {
                    case LuckyAppType.MinerTweak:
                        RpcRoot.Client.MinerTweakService.ShowMainWindowAsync((isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartLucky();
                            }
                            UIThread.Execute(() => app.Shutdown());
                        });
                        break;
                    case LuckyAppType.MinerMonitor:
                        RpcRoot.Client.MinerMonitorService.ShowMainWindowAsync((isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartLucky();
                            }
                            UIThread.Execute(() => app.Shutdown());
                        });
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) {
                RestartLucky();
                Logger.ErrorDebugLine(e);
            }
        }

        private void RestartLucky() {
            // 杀死其他进程后，这些进程会自动重启？
            Process thisProcess = Process.GetCurrentProcess();
            Windows.TaskKill.KillOtherProcess(thisProcess);
            Windows.Cmd.RunClose(VirtualRoot.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
        }
    }
}
