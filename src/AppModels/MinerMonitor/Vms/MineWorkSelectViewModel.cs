﻿using Lucky.Vms;
using System;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms
{
    public class MineWorkSelectViewModel : ViewModelBase
    {
        private MineWorkViewModel _selectedResult;
        private string _description;
        public readonly Action<MineWorkViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public MineWorkSelectViewModel()
        {
            if (!WpfUtil.IsInDesignMode)
            {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public MineWorkSelectViewModel(string description, MineWorkViewModel selected, Action<MineWorkViewModel> onOk)
        {
            _description = description;
            _selectedResult = selected;
            OnOk = onOk;
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public MineWorkViewModel SelectedResult
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

        public MinerMonitorRoot.MineWorkViewModels MineWorkVms
        {
            get
            {
                return MinerMonitorRoot.MineWorkVms;
            }
        }
    }
}
