using Lucky.Hub;
using System.Collections.ObjectModel;

namespace Lucky.Vms {
    public class MessagePathIdsViewModel : ViewModelBase {
        private readonly ObservableCollection<IMessagePathId> _pathIds;

        public MessagePathIdsViewModel() {
            _pathIds = new ObservableCollection<IMessagePathId>(VirtualRoot.MessageHub.GetAllPaths());
        }

        public ObservableCollection<IMessagePathId> PathIds {
            get {
                return _pathIds;
            }
        }
    }
}
