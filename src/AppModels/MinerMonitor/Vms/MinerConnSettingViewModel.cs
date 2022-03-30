using Lucky.Core.Daemon;
using Lucky.Vms;
using System;
using System.Windows;
using System.Windows.Input;


namespace Lucky.MinerMonitor.Vms
{
    public class MinerConnSettingViewModel : ViewModelBase
    {
        public readonly Guid Id = Guid.NewGuid();

        private int _selectedConnectionMethodIndex;
        private string _proxyServerAddress;
        private int _proxyServerPort;
        private string _proxyUsername;
        private string _proxyPassword;
        private bool _dirty = false;

        public ICommand Save { get; private set; }

        public int SelectedConnectionMethodIndex
        {
            get => _selectedConnectionMethodIndex;
            set
            {
                _dirty = true;
                _selectedConnectionMethodIndex = value;
                OnPropertyChanged(nameof(SelectedConnectionMethodIndex));
            }
        }

        public string ProxyServerAddress
        {
            get => _proxyServerAddress;
            set
            {
                _dirty = true;
                _proxyServerAddress = value;
                OnPropertyChanged(nameof(ProxyServerAddress));
            }
        }

        public int ProxyServerPort
        {
            get => _proxyServerPort;
            set
            {
                _dirty = true;
                _proxyServerPort = value;
                OnPropertyChanged(nameof(ProxyServerPort));
            }
        }

        public string ProxyUsername
        {
            get => _proxyUsername;
            set
            {
                _dirty = true;
                _proxyUsername = value;
                OnPropertyChanged(nameof(ProxyUsername));
            }
        }

        public string ProxyPassword
        {
            get => _proxyPassword;
            set
            {
                _dirty = true;
                _proxyPassword = value;
                OnPropertyChanged(nameof(ProxyPassword));
            }
        }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public MinerConnSettingViewModel()
        {
            if (!WpfUtil.IsInDesignMode)
            {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public MinerConnSettingViewModel(MinerViewModel[] minerClients)
        {
            if (minerClients != null && minerClients.Length == 1)
            {

                _selectedConnectionMethodIndex = minerClients[0].ConnectionMethod - 1;
                _proxyServerAddress = minerClients[0].ProxyServerAddress;
                _proxyServerPort = minerClients[0].ProxyServerPort;
                _proxyUsername = minerClients[0].ProxyUsername;
                _proxyPassword = minerClients[0].ProxyPassword;
            }
            this.Save = new DelegateCommand(() =>
            {

                if (minerClients != null && minerClients.Length != 0)
                {
                    foreach (var item in minerClients)
                    {
                        MinerMonitorRoot.MinerMonitorService.UpdateConnParamsAsync(item, new ConnParams()
                        {
                            ConnectionMethod = _selectedConnectionMethodIndex + 1,
                            ProxyServerAddress = _proxyServerAddress,
                            ProxyServerPort = _proxyServerPort,
                            ProxyUsername = _proxyUsername,
                            ProxyPassword = _proxyPassword
                        });
                    }
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            }, IsDirty);
        }

        private bool IsDirty ()
        {
            return _dirty;
        }

    }
}
