using System.Windows;

namespace Lucky.Controls {
    public class KbSystemCloseButton : KbSystemButton {
        private Window targetWindow;
        public KbSystemCloseButton() {
            KbSystemButtonHoverBackground = WpfUtil.RedBrush;

            this.Command = new DelegateCommand(()=> {
                if (targetWindow == null) {
                    targetWindow = Window.GetWindow(this);
                }
                targetWindow.Close();
            });
        }
    }
}
