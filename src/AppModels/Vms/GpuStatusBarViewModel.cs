﻿using Lucky.Gpus;
using System.Linq;
using System.Windows.Media;

namespace Lucky.Vms {
    public class GpuStatusBarViewModel : ViewModelBase {
        public static GpuStatusBarViewModel Instance { get; private set; } = new GpuStatusBarViewModel();

        private GpuStatusBarViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.GpuAllVm = AppRoot.GpuVms.Items.FirstOrDefault(a => a.Index == LuckyContext.GpuAllId);
        }

        public GpuViewModel GpuAllVm {
            get; set;
        }

        private Geometry _icon;
        public Geometry Icon {
            get {
                if (_icon == null) {
                    string iconName;
                    GpuType gpuType = GpuType.NVIDIA;
                    if (!WpfUtil.IsInDesignMode) {
                        gpuType = LuckyContext.Instance.GpuSet.GpuType;
                    }
                    switch (gpuType) {
                        case GpuType.NVIDIA:
                            iconName = "Icon_Nvidia";
                            break;
                        case GpuType.AMD:
                            iconName = "Icon_AMD";
                            break;
                        default:
                            iconName = "Icon_GpuEmpty";
                            break;
                    }
                    _icon = AppUtil.GetResource<Geometry>(iconName);
                }
                return _icon;
            }
        }

        public string IconFill {
            get {
                string iconFill;
                GpuType gpuType = GpuType.NVIDIA;
                if (!WpfUtil.IsInDesignMode) {
                    gpuType = LuckyContext.Instance.GpuSet.GpuType;
                }
                switch (gpuType) {
                    case GpuType.NVIDIA:
                        iconFill = "Green";
                        break;
                    case GpuType.AMD:
                        iconFill = "Red";
                        break;
                    case GpuType.Empty:
                    default:
                        iconFill = "Gray";
                        break;
                }
                return iconFill;
            }
        }

        public string GpuSetName {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return string.Empty;
                }
                if (LuckyContext.Instance.GpuSet.GpuType.IsEmpty()) {
                    return "没有矿卡或矿卡未驱动";
                }
                return LuckyContext.Instance.GpuSet.GpuType.GetDescription();
            }
        }

        public string GpuSetInfo {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "p106-100 x 8";
                }
                return LuckyContext.Instance.GpuSetInfo;
            }
        }
    }
}
