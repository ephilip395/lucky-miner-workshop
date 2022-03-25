using System.Windows;
using Lucky.Microsoft.Windows.Shell;

namespace Lucky {
    /// <summary>
    /// This class eats little children.
    /// </summary>
    internal static class BlankWindowHelpers {
        /// <summary>
        /// Sets the IsHitTestVisibleInChromeProperty to a MetroWindow template child
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="name">The name of the template child</param>
        public static void SetIsHitTestVisibleInChromeProperty<T>(this BlankWindow window, string name) where T : DependencyObject {
            if (window == null) {
                return;
            }
            T elementPart = window.GetPart<T>(name);
            if (elementPart != null) {
                elementPart.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
        }
    }
}
