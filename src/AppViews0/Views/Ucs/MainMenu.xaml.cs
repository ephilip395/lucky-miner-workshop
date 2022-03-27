using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs
{
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            this.DataContext = MainMenuViewModel.Instance;
            InitializeComponent();
        }

        private void MenuItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VirtualRoot.Execute(new TopmostCommand());
        }

        private void MenuItem_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VirtualRoot.Execute(new UnTopmostCommand());
        }
    }
}
