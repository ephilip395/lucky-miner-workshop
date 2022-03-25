using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace Lucky.DivertProxyNu
{
    public static class DivertProxyNuUtil
    {
        public static void RunLuckyDivertProxyNu()
        {
            if (ClientAppType.IsMinerMonitor)
            {
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(LuckyKeyword.DivertProxyNuFileName);
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0)
            {
                string thatVersion = LuckyRegistry.GetDivertProxyNuVersion();
                try
                {
                    string thisVersion = ThisLuckyDivertProxyNuFileVersion;
                    if (thatVersion != thisVersion)
                    {
                        Logger.InfoDebugLine($"发现新版 DivertProxyNu：{thatVersion}->{thisVersion}");
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                        System.Threading.Thread.Sleep(1000);
                        ExtractRunLuckyDivertProxyNuAsync();
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                }
            }
            else
            {
                ExtractRunLuckyDivertProxyNuAsync();
            }
        }

        private static void ExtractRunLuckyDivertProxyNuAsync()
        {
            Task.Factory.StartNew(() =>
            {
                ExtractResource(LuckyKeyword.DivertProxyNuFileName);
                Windows.Cmd.RunClose(MinerTweakTempPath.DivertProxyNuFileFullName, string.Empty, waitForExit: true, createNoWindow: !DevMode.IsDevMode);
                Logger.OkDebugLine("DivertProxyNu 进程启动成功");
            });
        }

        private static void ExtractResource(string name)
        {
            try
            {
                Type type = typeof(DivertProxyNuUtil);
                Assembly assembly = type.Assembly;
                string DivertProxyNuDir = Path.GetDirectoryName(MinerTweakTempPath.DivertProxyNuFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(DivertProxyNuDir, name));
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisLuckyDivertProxyNuFileVersion;
        private static string ThisLuckyDivertProxyNuFileVersion
        {
            get
            {
                if (s_thisLuckyDivertProxyNuFileVersion == null)
                {
                    try
                    {
                        string name = "sha1";
                        Type type = typeof(DivertProxyNuUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name))
                        {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisLuckyDivertProxyNuFileVersion = System.Text.Encoding.UTF8.GetString(data);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorDebugLine(e);
                        s_thisLuckyDivertProxyNuFileVersion = string.Empty;
                    }
                }
                return s_thisLuckyDivertProxyNuFileVersion;
            }
        }
    }
}
