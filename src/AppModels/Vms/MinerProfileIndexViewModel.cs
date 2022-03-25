using System.Collections.Generic;
using System.Linq;

namespace Lucky.Vms {
    public class MinerProfileIndexViewModel : ViewModelBase {
        public MinerProfileIndexViewModel() {
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
            }
        }

        public List<GpuViewModel> GpuVms {
            get {
                return AppRoot.GpuVms.Items.Where(a => a.Index != LuckyContext.GpuAllId).OrderBy(a => a.Index).ToList();
            }
        }
    }
}
