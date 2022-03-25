using Lucky.Vms;
using System;
using System.Windows.Controls;

namespace Lucky.Views.Ucs
{
    /// <summary>
    /// Interaction logic for Output.xaml
    /// </summary>
    public partial class Output : UserControl
    {
        private OutputViewModel Vm { get; }
        public Output()
        {

            Vm = new OutputViewModel();
            DataContext = Vm;
            InitializeComponent();
            LuckyConsole.PartnerWriter = Vm;
        }
    }
}
