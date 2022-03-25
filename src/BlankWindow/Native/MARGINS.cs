using System.Runtime.InteropServices;

namespace Lucky.Native {
    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGINS {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }
}