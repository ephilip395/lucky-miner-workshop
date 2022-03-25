using System.Windows.Input;

namespace Lucky.Vms {
    public interface IEditableViewModel {
        ICommand Edit { get; }
    }
}
