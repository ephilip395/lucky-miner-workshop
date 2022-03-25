using Lucky.Core;
using Lucky.Net;
using System;
using System.Windows.Input;

namespace Lucky.Vms
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand CloseMainWindow { get; private set; }

        public MainWindowViewModel()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            this.CloseMainWindow = new DelegateCommand(() =>
            {
                VirtualRoot.Execute(new CloseMainWindowCommand(isAutoNoUi: false));
            });
        }

        public bool IsTestHost
        {
            get
            {
                return !string.IsNullOrEmpty(Hosts.GetIp(RpcRoot.OfficialServerHost, out long _));
            }
            set
            {
                if (value)
                {
                    Hosts.SetHost(RpcRoot.OfficialServerHost, "127.0.0.1");
                }
                else
                {
                    Hosts.SetHost(RpcRoot.OfficialServerHost, string.Empty);
                }
                OnPropertyChanged(nameof(IsTestHost));
            }
        }

        public string BrandTitle
        {
            get
            {
                if (LuckyContext.KernelBrandId == Guid.Empty && LuckyContext.PoolBrandId == Guid.Empty)
                {
                    return string.Empty;
                }
                if (LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(LuckyContext.KernelBrandId, out ISysDicItem dicItem))
                {
                    if (!string.IsNullOrEmpty(dicItem.Value))
                    {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (LuckyContext.Instance.ServerContext.SysDicItemSet.TryGetDicItem(LuckyContext.PoolBrandId, out dicItem))
                {
                    if (!string.IsNullOrEmpty(dicItem.Value))
                    {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public MinerProfileViewModel MinerProfile
        {
            get
            {
                return AppRoot.MinerProfileVm;
            }
        }
    }
}
