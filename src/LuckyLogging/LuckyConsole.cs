using Lucky.Core;
using Lucky.Impl;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lucky
{
    public static class LuckyConsole
    {
        private static class SafeNativeMethods
        {
            [DllImport(DllName.Kernel32Dll)]
            private static extern bool AllocConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern bool FreeConsole();
            [DllImport(DllName.Kernel32Dll)]
            internal static extern IntPtr GetConsoleWindow();
            [DllImport(DllName.User32Dll, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
            internal static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
            [DllImport(DllName.User32Dll, SetLastError = true)]
            internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern IntPtr GetStdHandle(int hConsoleHandle);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
            [DllImport(DllName.Kernel32Dll, SetLastError = true)]
            private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

            internal static void DisbleQuickEditMode()
            {
                const int STD_INPUT_HANDLE = -10;
                const uint ENABLE_PROCESSED_INPUT = 0x0001;
                const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
                const uint ENABLE_INSERT_MODE = 0x0020;

                IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
                _ = GetConsoleMode(hStdin, out uint mode);
                mode &= ~ENABLE_PROCESSED_INPUT; // 禁用ctrl+c
                mode &= ~ENABLE_QUICK_EDIT_MODE; // 移除快速编辑模式
                mode &= ~ENABLE_INSERT_MODE;     // 移除插入模式
                _ = SetConsoleMode(hStdin, mode);
            }

            internal static IntPtr GetOrAlloc(bool disableQuickEditMode)
            {
                IntPtr console = GetConsoleWindow();
                if (console == IntPtr.Zero)
                {
                    _ = AllocConsole();
                    if (disableQuickEditMode)
                    {
                        DisbleQuickEditMode();
                    }
                    console = GetConsoleWindow();
                    Console.WriteLine();
                    PartnerWriter?.WriteLine();
                }
                return console;
            }
        }

        private static readonly object _locker = new object();
        private static readonly List<Tuple<string, ConsoleColor>> _lineBeforeMainUiOk = new List<Tuple<string, ConsoleColor>>();
        private static readonly object _lockForUserLine = new object();
        private static readonly DateTime _unixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static bool _isInited = false;
        private static int _uiThreadId;
        private static bool _isMainUiOk = false;
        private static ConsoleColor _foreground = ConsoleColor.White;

        private static bool IsMainUiOk => DevMode.IsDevMode || _isMainUiOk;

        public static bool Enabled { get; set; } = true;
        // 缓存最近的几十条输出行
        public static readonly IConsoleOutLineSet ConsoleOutLineSet = new ConsoleOutLineSet();

        public static IOutputWriter PartnerWriter { get; set; } = null;

        public static void Init(bool initHide = false)
        {
            if (!_isInited)
            {
                lock (_locker)
                {
                    if (!_isInited)
                    {
                        _isInited = true;
                        IntPtr hWnh = GetOrAlloc(false);
                        if (initHide)
                        {
                            _ = SafeNativeMethods.ShowWindow(hWnh, 0);
                        }
                    }
                }
            }
        }

        public static void SetUIThreadId(int value)
        {
            _uiThreadId = value;
        }

        public static void MainUiOk()
        {
            _isMainUiOk = true;
            WriteLinesBeforeMainUiOk();
        }

        /// <summary>
        /// 禁止编辑控制台窗口
        /// </summary>
        public static void DisbleQuickEditMode()
        {
            SafeNativeMethods.DisbleQuickEditMode();
        }

        public static IntPtr GetOrAlloc(bool disableQuickEditMode = true)
        {
            return SafeNativeMethods.GetOrAlloc(disableQuickEditMode);
        }

        /// <summary>
        /// 如果程序没有控制台窗口调用没有副作用，应在程序生命周期最末尾调用。
        /// 因为控制台窗口是按需创建的，所以不能在顺序不定的AppExit中释放，以免释放后又按需创建。
        /// </summary>
        public static void Free()
        {
            IntPtr console = SafeNativeMethods.GetConsoleWindow();
            if (console != IntPtr.Zero)
            {
                _ = SafeNativeMethods.FreeConsole();
            }
        }

        private static long GetTimestamp()
        {
            return (long)(DateTime.Now.ToUniversalTime() - _unixBaseTime).TotalSeconds;
        }

        private static readonly Action<string, ConsoleColor> _userLineMethod = (line, color) =>
        {
            if (!Enabled)
            {
                return;
            }
            if (!IsMainUiOk)
            {
                _lineBeforeMainUiOk.Add(new Tuple<string, ConsoleColor>(line, color));
                return;
            }
            WriteLinesBeforeMainUiOk();
            lock (_lockForUserLine)
            {
                ConsoleColor oldColor = _foreground;
                _foreground = color;

                if (_isInited)
                {
                    Console.ForegroundColor = _foreground;
                    Console.WriteLine(line);
                    Console.ResetColor();
                }

                PartnerWriter?.WriteLine(line, _foreground);

                _foreground = oldColor;
            }
        };

        private static void WriteLinesBeforeMainUiOk()
        {
            if (!IsMainUiOk)
            {
                return;
            }
            lock (_lockForUserLine)
            {
                if (_lineBeforeMainUiOk.Count != 0)
                {
                    foreach (Tuple<string, ConsoleColor> item in _lineBeforeMainUiOk)
                    {
                        if (_isInited)
                        {
                            Console.ForegroundColor = item.Item2;
                            Console.WriteLine(item.Item1);
                            Console.ResetColor();
                        }
                        PartnerWriter?.WriteLine(item.Item1, item.Item2);
                    }

                    _lineBeforeMainUiOk.Clear();
                }
            }
        }

        public static void UserLine(string text, MessageType messageType = MessageType.Default)
        {
            if (messageType == MessageType.Debug && !DevMode.IsDevMode)
            {
                return;
            }
            UserLine($"Lucky {messageType,-10}  {text}", messageType.ToConsoleColor());
        }

        public static void UserLine(string text, string messageType, ConsoleColor color)
        {
            UserLine($"Lucky {messageType,-10}  {text}", color);
        }

        public static void UserError(string text)
        {
            UserLine(text, MessageType.Error);
        }

        public static void UserInfo(string text)
        {
            UserLine(text, MessageType.Info);
        }

        public static void UserOk(string text)
        {
            UserLine(text, MessageType.Ok);
        }

        public static void UserWarn(string text)
        {
            UserLine(text, MessageType.Warn);
        }

        public static void UserFail(string text)
        {
            UserLine(text, MessageType.Fail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="foreground"></param>
        /// <param name="withPrefix">表示是否在打印的行前附加个时间戳，默认附加</param>
        public static void UserLine(string line, ConsoleColor foreground, bool withPrefix = true)
        {
            if (!Enabled)
            {
                return;
            }
            if (withPrefix)
            {
                line = $"{DateTime.Now:HH:mm:ss fff}  {(Thread.CurrentThread.ManagedThreadId == _uiThreadId ? "UI" : "  ")} {line}";
            }
            ConsoleOutLineSet.Add(new ConsoleOutLine
            {
                Timestamp = GetTimestamp(),
                Line = line
            });
            _userLineMethod?.Invoke(line, foreground);
        }


        public static void DevLine(string text, MessageType messageType = MessageType.Default, ConsoleColor foreground = default)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            text = $"{DateTime.Now:HH:mm:ss fff}  {(Thread.CurrentThread.ManagedThreadId == _uiThreadId ? "UI" : "  ")} {messageType.ToString()} {text}";

            ConsoleColor oldColor = _foreground;
            _foreground = foreground != default ? _foreground = foreground : messageType.ToConsoleColor();

            if (_isInited)
            {
                Console.ForegroundColor = _foreground;
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }

            PartnerWriter?.WriteLine(text, _foreground);
            _foreground = oldColor;

        }

        public static void DevException(Exception e)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            DevLine(e.GetInnerMessage() + e.StackTrace, MessageType.Error);
        }

        public static void DevException(string message, Exception e)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            DevLine(message + e.StackTrace, MessageType.Error);
        }

        public static void DevError(string text)
        {
            DevLine(text, MessageType.Error);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevError(Func<string> getText)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            DevLine(getText(), MessageType.Error);
        }

        public static void DevDebug(string text)
        {
            DevLine(text, MessageType.Debug);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevDebug(Func<string> getText, ConsoleColor foreground = default)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            if (getText == null)
            {
                return;
            }
            DevLine(getText(), MessageType.Debug, foreground);
        }

        public static void DevOk(string text)
        {
            DevLine(text, MessageType.Ok);
        }

        /// <summary>
        /// 延迟参数计算。
        /// 有一个特殊情况需要注意：不要在Rpc Fire回调中使用，因为Rpc Fire回调中访问getHttpResponse.Result.ReasonPhrase的目的就是为了触发计算，所以不能延迟计算。
        /// </summary>
        /// <param name="getText"></param>
        public static void DevOk(Func<string> getText)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            if (getText == null)
            {
                return;
            }
            DevLine(getText(), MessageType.Ok);
        }

        public static void DevWarn(string text)
        {
            DevLine(text, MessageType.Warn);
        }

        public static void DevWarn(Func<string> getText)
        {
            if (!DevMode.IsDevMode)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }
            if (getText == null)
            {
                return;
            }
            DevLine(getText(), MessageType.Warn);
        }

#if DEBUG
        public static void DevTimeSpan(string text)
        {
            DevLine(text, MessageType.TimeSpan);
        }
#endif

        public static void DevFail(string text)
        {
            DevLine(text, MessageType.Fail);
        }
    }
}
