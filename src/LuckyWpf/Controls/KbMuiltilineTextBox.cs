using System.Windows.Controls;

namespace Lucky.Controls {
    public class KbMuiltilineTextBox : TextBox {
        public KbMuiltilineTextBox() {
            this.AcceptsReturn = true;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
            this.Background = WpfUtil.TransparentBrush;
        }
    }
}
