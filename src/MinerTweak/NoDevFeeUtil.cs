using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace Lucky.NoDevFee
{
    public static class NoDevFeeUtil
    {
        public static void RunLuckyNoDevFee()
        {
            if (ClientAppType.IsMinerMonitor)
            {
                return;
            }
            string processName = Path.GetFileNameWithoutExtension(LuckyKeyword.LuckyNoDevFeeFileName);
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0)
            {
                string thatVersion = LuckyRegistry.GetNoDevFeeVersion();
                try
                {
                    string thisVersion = ThisLuckyNoDevFeeFileVersion;
                    if (thatVersion != thisVersion)
                    {
                        Logger.InfoDebugLine($"发现新版NoDevFee：{thatVersion}->{thisVersion}");
                        Windows.TaskKill.Kill(processName, waitForExit: true);
                        System.Threading.Thread.Sleep(1000);
                        ExtractRunLuckyNoDevFeeAsync();
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorDebugLine(e);
                }
            }
            else
            {
                ExtractRunLuckyNoDevFeeAsync();
            }
        }

        private static void ExtractRunLuckyNoDevFeeAsync()
        {
            Task.Factory.StartNew(() =>
            {
                ExtractResource(LuckyKeyword.LuckyNoDevFeeFileName);
                Windows.Cmd.RunClose(MinerTweakTempPath.NoDevFeeFileFullName, string.Empty, waitForExit: true, createNoWindow: !DevMode.IsDevMode);
                Logger.OkDebugLine("NoDevFee 进程启动成功");
            });
        }

        private static void ExtractResource(string name)
        {
            try
            {
                Type type = typeof(NoDevFeeUtil);
                Assembly assembly = type.Assembly;
                string noDevFeeDir = Path.GetDirectoryName(MinerTweakTempPath.NoDevFeeFileFullName);
                assembly.ExtractManifestResource(type, name, Path.Combine(noDevFeeDir, name));
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string s_thisLuckyNoDevFeeFileVersion;
        private static string ThisLuckyNoDevFeeFileVersion
        {
            get
            {
                if (s_thisLuckyNoDevFeeFileVersion == null)
                {
                    try
                    {
                        string name = "sha1";
                        Type type = typeof(NoDevFeeUtil);
                        Assembly assembly = type.Assembly;
                        using (var stream = assembly.GetManifestResourceStream(type, name))
                        {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);
                            s_thisLuckyNoDevFeeFileVersion = System.Text.Encoding.UTF8.GetString(data);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorDebugLine(e);
                        s_thisLuckyNoDevFeeFileVersion = string.Empty;
                    }
                }
                return s_thisLuckyNoDevFeeFileVersion;
            }
        }
    }
}
