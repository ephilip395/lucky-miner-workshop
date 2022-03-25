using System;

namespace Lucky {
    public class Program {
        [STAThread()]
        public static void Main() {
            AppUtil.Run<App>(withSplashWindow: true);
        }
    }
}
