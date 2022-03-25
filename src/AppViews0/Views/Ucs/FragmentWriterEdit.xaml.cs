using Lucky.Vms;
using System.Windows.Controls;

namespace Lucky.Views.Ucs {
    public partial class FragmentWriterEdit : UserControl {
        public static void ShowWindow(FormType formType, FragmentWriterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "命令行片段书写器",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 950,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_FragmentWriter"
            }, ucFactory: (window) =>
            {
                FragmentWriterViewModel vm = new FragmentWriterViewModel(source);
                window.BuildCloseWindowOncePath(vm.Id);
                return new FragmentWriterEdit(vm);
            }, fixedSize: true);
        }

        public FragmentWriterViewModel Vm { get; private set; }

        public FragmentWriterEdit(FragmentWriterViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
