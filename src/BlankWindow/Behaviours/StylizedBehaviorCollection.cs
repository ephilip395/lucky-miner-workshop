using System.Windows;
using System.Windows.Interactivity;

namespace Lucky.Behaviours {
    public class StylizedBehaviorCollection : FreezableCollection<Behavior> {
        protected override Freezable CreateInstanceCore() {
            return new StylizedBehaviorCollection();
        }
    }
}