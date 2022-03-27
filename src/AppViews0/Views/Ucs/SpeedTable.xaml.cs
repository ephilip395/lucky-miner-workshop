using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs
{
    public partial class SpeedTable : UserControl
    {
        public SpeedTableViewModel Vm { get; private set; }

        public SpeedTable()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            this.Vm = new SpeedTableViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this).DragMove();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
