using Lucky.Core.MinerServer;
using Lucky.Vms;
using System;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms
{
    public class LuckyWalletViewModel : ViewModelBase, ILuckyWallet, IEditableViewModel
    {
        private Guid _id;
        private string _coinCode;
        private string _wallet;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public LuckyWalletViewModel()
        {
            if (!WpfUtil.IsInDesignMode)
            {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public LuckyWalletViewModel(Guid id)
        {
            _id = id;
            this.Save = new DelegateCommand(() =>
            {
                if (this.Id == Guid.Empty)
                {
                    return;
                }
                if (LuckyContext.MinerMonitorContext.LuckyWalletSet.TryGetLuckyWallet(Id, out ILuckyWallet _))
                {
                    VirtualRoot.Execute(new UpdateLuckyWalletCommand(this));
                }
                else
                {
                    VirtualRoot.Execute(new AddLuckyWalletCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommandTpl<FormType?>((formType) =>
            {
                if (this.Id == Guid.Empty)
                {
                    return;
                }
                VirtualRoot.Execute(new EditLuckyWalletCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() =>
            {
                if (this.Id == Guid.Empty)
                {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除吗？", title: "确认", onYes: () =>
                {
                    VirtualRoot.Execute(new RemoveLuckyWalletCommand(this.Id));
                }));
            });
        }

        public LuckyWalletViewModel(ILuckyWallet data) : this(data.GetId())
        {
            _coinCode = data.CoinCode;
            _wallet = data.Wallet;
        }

        public Guid GetId()
        {
            return Id;
        }

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string CoinCode
        {
            get => _coinCode;
            set
            {
                _coinCode = value;
                OnPropertyChanged(nameof(CoinCode));
            }
        }

        public string Wallet
        {
            get => _wallet;
            set
            {
                _wallet = value;
                OnPropertyChanged(nameof(Wallet));
            }
        }

        public MainMenuViewModel MainMenu
        {
            get
            {
                return MainMenuViewModel.Instance;
            }
        }
    }
}
