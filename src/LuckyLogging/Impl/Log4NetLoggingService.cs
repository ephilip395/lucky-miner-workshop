using log4net;
using log4net.Config;
using System;
using System.IO;

namespace Lucky.Impl
{
    public sealed class Log4NetLoggingService : ILoggingService
    {
        private readonly ILog _log;

        public Log4NetLoggingService()
        {
            if (string.IsNullOrEmpty(Logger.DirFullPath))
            {
                throw new InvalidProgramException();
            }
            string logFile = Path.Combine(Logger.DirFullPath, GetLogFileName());
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(
$@"<log4net>
  <root>
    <level value=""ALL"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
  <appender name=""RollingFileAppender"" type =""log4net.Appender.RollingFileAppender"" >
    <filter type=""log4net.Filter.LevelRangeFilter"" >
    </filter>
    <param name=""File"" value =""{logFile}"" />
    <param name=""AppendToFile"" value =""true"" />
    <param name=""MaxSizeRollBackups"" value =""10"" />
    <param name=""MaximumFileSize"" value =""2MB"" />
    <param name=""RollingStyle"" value =""Size"" />
    <param name=""StaticLogFileName"" value =""true"" />
    <layout type=""log4net.Layout.PatternLayout"" >
      <param name=""ConversionPattern"" value =""%d [%t] %-5p %c - %m%n"" />
    </layout>
  </appender>
</log4net>");
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                _ = XmlConfigurator.Configure(ms);
            }
            _log = LogManager.GetLogger("global");
        }

        /// <summary>
        /// 返回的日志文件名和应用程序的类型和版本以及启动参数有关系。
        /// 主要是因为挖矿端应用程序不是单例的，也就是说已经有一个挖矿端程序进程时当挖矿端程序启动时追加了比如upgrade=luckycm2.8.exe参数的话依旧可以启动进程。
        /// </summary>
        /// <returns></returns>
        private static string GetLogFileName()
        {
            // 避免不同进程使用相同的日志文件，虽然并不会异常但会看不到日志
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade))
            {
                return $"root{LuckyKeyword.AssemblyVersion}_upgrade.log";
            }
            else if (!string.IsNullOrEmpty(CommandLineArgs.Action))
            {
                return $"root{LuckyKeyword.AssemblyVersion}_{CommandLineArgs.Action}.log";
            }
            return $"root{LuckyKeyword.AssemblyVersion}.log";
        }

        public void Debug(object message)
        {
            LuckyConsole.DevLine(message?.ToString());
            _log.Debug(message);
        }

        public void InfoDebugLine(object message)
        {
            LuckyConsole.DevDebug(message?.ToString());
            _log.Info(message);
        }

        public void OkDebugLine(object message)
        {
            LuckyConsole.DevOk(message?.ToString());
            _log.Info(message);
        }

        public void WarnDebugLine(object message)
        {
            LuckyConsole.DevWarn(message?.ToString());
            _log.Warn(message);
        }

        public void ErrorDebugLine(object message)
        {
            LuckyConsole.DevError(message?.ToString());
            _log.Error(message);
        }

        public void ErrorDebugLine(object message, Exception exception)
        {
            LuckyConsole.DevError(message?.ToString() + exception.StackTrace);
            _log.Error(message, exception);
        }

        public void OkUserLine(object message)
        {
            LuckyConsole.UserOk(message?.ToString());
            _log.Info(message);
        }

        public void WarnUserLine(object message)
        {
            LuckyConsole.UserWarn(message?.ToString());
            _log.Warn(message);
        }

        public void ErrorUserLine(object message)
        {
            LuckyConsole.UserError(message?.ToString());
            _log.Warn(message);
        }
    }
}
