using Lucky.Core.MinerServer;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms {
    public class LuckyFileSelectViewModel : ViewModelBase {
        private List<LuckyFileViewModel> _luckycmFileVms;

        private LuckyFileViewModel _selectedResult;
        public readonly Action<LuckyFileViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public LuckyFileSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public LuckyFileSelectViewModel(Action<LuckyFileViewModel> onOk) {
            OnOk = onOk;
            _selectedResult = null;
            _luckycmFileVms = new List<LuckyFileViewModel>();
            // 因为LuckyFiles列表是异步初始化的，下面填充几个空对象的目的是解决WPFpopup的某个BUG，否则第一次打开popup的时候位置不对。
            for (int i = 0; i < 7; i++) {
                _luckycmFileVms.Add(LuckyFileViewModel.Empty);
            }
            VirtualRoot.BuildEventPath<LuckyFileSetInitedEvent>("行运矿工程序版本文件集初始化后刷新Vm内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                var luckycmFiles = MinerMonitorRoot.ReadOnlyLuckyFileSet.AsEnumerable().Where(a => a.AppType == LuckyAppType.MinerTweak);
                LuckyFileVms = luckycmFiles.OrderByDescending(a => a.GetVersion()).Select(a => new LuckyFileViewModel(a)).ToList();
            });
            // 触发从远程加载数据的逻辑
            VirtualRoot.Execute(new RefreshLuckyFileSetCommand());
        }

        public LuckyFileViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<LuckyFileViewModel> LuckyFileVms {
            get { return _luckycmFileVms; }
            set {
                _luckycmFileVms = value;
                OnPropertyChanged(nameof(LuckyFileVms));
            }
        }
    }
}
