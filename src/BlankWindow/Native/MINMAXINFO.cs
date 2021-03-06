using System.Runtime.InteropServices;

namespace Lucky.Native {
    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };
}