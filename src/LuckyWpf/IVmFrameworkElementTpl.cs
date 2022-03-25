using System.Windows;

namespace Lucky {
    public interface IVmFrameworkElementTpl<TVm> {
        TVm Vm { get; set; }
        object DataContext { get; set; }
        event DependencyPropertyChangedEventHandler DataContextChanged;
    }
}
