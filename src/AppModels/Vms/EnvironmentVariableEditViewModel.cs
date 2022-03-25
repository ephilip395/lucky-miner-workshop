﻿using Lucky.Core;
using System;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms {
    public class EnvironmentVariableEditViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private string _key;
        private string _value;

        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public EnvironmentVariableEditViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public EnvironmentVariableEditViewModel(CoinKernelViewModel coinKernelViewModel, EnvironmentVariable environmentVariable) {
            _key = environmentVariable.Key;
            _value = environmentVariable.Value;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Key)) {
                    throw new ValidationException("变量名不能为空");
                }
                environmentVariable.Key = this.Key;
                environmentVariable.Value = this.Value;
                if (!coinKernelViewModel.EnvironmentVariables.Contains(environmentVariable)) {
                    coinKernelViewModel.EnvironmentVariables.Add(environmentVariable);
                }
                coinKernelViewModel.EnvironmentVariables = coinKernelViewModel.EnvironmentVariables.ToList();
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
        }

        public string Key {
            get { return _key; }
            set {
                _key = value;
                OnPropertyChanged(nameof(Key));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("变量名不能为空");
                }
            }
        }

        public string Value {
            get { return _value; }
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
