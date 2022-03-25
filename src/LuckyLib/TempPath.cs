using System;
using System.IO;

namespace Lucky
{
    public static class TempPath
    {
        public static readonly string TempDirFullName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            LuckyKeyword.TempDirName);

        private static string s_tempLogsDirFullName = null;
        public static string TempLogsDirFullName
        {
            get
            {
                if (s_tempLogsDirFullName == null)
                {
                    s_tempLogsDirFullName = Path.Combine(TempDirFullName, LuckyKeyword.LogsDirName);
                    if (!Directory.Exists(s_tempLogsDirFullName))
                    {
                        _ = Directory.CreateDirectory(s_tempLogsDirFullName);
                    }
                }

                return s_tempLogsDirFullName;
            }
        }
    }
}
