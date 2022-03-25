using Lucky.Core.MinerTweak;
using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms
{
    public class LocalIpConfigViewModel : ViewModelBase
    {
        public readonly Guid Id = Guid.NewGuid();
        private List<LocalIpViewModel> _localIpVms = new List<LocalIpViewModel>();
        private bool _isLoading = true;

        public ICommand Save { get; private set; }

        public LocalIpConfigViewModel(MinerTweakViewModel minerClientVm)
        {
            this.MinerTweakVm = minerClientVm;
            this.Save = new DelegateCommandTpl<LocalIpViewModel>((vm) =>
            {
                if (!vm.IsAutoDNSServer)
                {
                    if (vm.DNSServer0Vm.IsAnyEmpty)
                    {
                        vm.DNSServer0Vm.SetAddress(LuckyKeyword.DNSServer0);
                    }
                    if (vm.DNSServer1Vm.IsAnyEmpty)
                    {
                        vm.DNSServer1Vm.SetAddress(LuckyKeyword.DNSServer1);
                    }
                }
                MinerMonitorRoot.MinerMonitorService.SetLocalIpsAsync(minerClientVm, _localIpVms.Select(a => LocalIpInput.Create(a, a.IsAutoDNSServer)).ToList());
                if (_localIpVms.Count == 1)
                {
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                }
            }, (vm) => vm.IsChanged);
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public MinerTweakViewModel MinerTweakVm
        {
            get; private set;
        }

        public string DNSServer0Tooltip
        {
            get
            {
                return "留空表示使用腾讯：" + LuckyKeyword.DNSServer0;
            }
        }

        public string DNSServer1Tooltip
        {
            get
            {
                return "留空表示使用阿里：" + LuckyKeyword.DNSServer1;
            }
        }

        public List<LocalIpViewModel> LocalIpVms
        {
            get => _localIpVms;
            set
            {
                _localIpVms = value;
                OnPropertyChanged(nameof(LocalIpVms));
                this.IsLoading = false;
            }
        }
    }
}
