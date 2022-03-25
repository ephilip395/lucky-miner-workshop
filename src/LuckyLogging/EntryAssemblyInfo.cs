﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lucky
{
    /// <summary>
    /// 这里是一些从入口程序集提取的一些信息，比如入口程序集的版本号。
    /// </summary>
    public static class EntryAssemblyInfo
    {
        public static readonly Version CurrentVersion;
        public static readonly string CurrentVersionStr;
        public static readonly string CurrentVersionTag = string.Empty;

        static EntryAssemblyInfo()
        {
            if (!DevMode.IsInUnitTest)
            {
                Assembly mainAssembly = Assembly.GetEntryAssembly();
                CurrentVersion = mainAssembly.GetName().Version;
                AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), inherit: false).FirstOrDefault();
                CurrentVersionTag = description?.Description;
            }
            else
            {
                CurrentVersion = new Version(2, 0, 0, 0);
                CurrentVersionTag = "UnitTest";
            }
            CurrentVersionStr = CurrentVersion.ToString(4);
        }

        public static void ExtractManifestResource(this Assembly assembly, Type type, string name, string saveFileFuleName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(type, name))
            {
                byte[] data = new byte[stream.Length];
                _ = stream.Read(data, 0, data.Length);
                File.WriteAllBytes(saveFileFuleName, data);
            }
        }
    }
}
