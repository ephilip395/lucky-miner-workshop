﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms
{
    public class WalletSelectViewModel : ViewModelBase
    {
        private CoinViewModel _coin;
        private readonly bool _isDualCoin;
        private WalletViewModel _selectedResult;
        public readonly Action<WalletViewModel> OnOk;

        public ICommand AddWallet { get; private set; }
        public ICommand HideView { get; set; }

        public WalletSelectViewModel(CoinViewModel coin, bool isDualCoin, WalletViewModel selected, Action<WalletViewModel> onOk)
        {
            _coin = coin;
            _isDualCoin = isDualCoin;
            _selectedResult = selected;
            OnOk = onOk;
            this.AddWallet = new DelegateCommand(() =>
            {
                this.HideView.Execute(null);
                if (_isDualCoin)
                {
                    _coin.CoinProfile?.AddDualCoinWallet.Execute(null);
                }
                else
                {
                    _coin.CoinProfile?.AddWallet.Execute(null);
                }
            });
        }

        public WalletViewModel SelectedResult
        {
            get => _selectedResult;
            set
            {
                if (_selectedResult != value)
                {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public CoinViewModel Coin
        {
            get => _coin;
            set
            {
                if (_coin != value)
                {
                    _coin = value;
                    OnPropertyChanged(nameof(Coin));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
        }

        public List<WalletViewModel> QueryResults
        {
            get
            {
                return Coin.Wallets.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
