﻿using Lucky.Vms;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms {
    public class MinerNamesSeterViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private List<Tuple<string, string>> _namesByObjectId;
        private string _prefix;
        private string _suffix;

        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public MinerNamesSeterViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public MinerNamesSeterViewModel(string prefix, string suffix, List<Tuple<string, string>> namesByObjectId, Action onOk) {
            _prefix = prefix;
            _suffix = suffix;
            _namesByObjectId = namesByObjectId;
            RefreshNames();
            this.Save = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                onOk?.Invoke();
            });
        }

        public string Prefix {
            get => _prefix;
            set {
                _prefix = value;
                OnPropertyChanged(nameof(Prefix));
                RefreshNames();
            }
        }
        public string Suffix {
            get => _suffix;
            set {
                _suffix = value;
                OnPropertyChanged(nameof(Suffix));
                RefreshNames();
            }
        }

        public List<Tuple<string, string>> NamesByObjectId {
            get => _namesByObjectId;
            set {
                _namesByObjectId = value;
                OnPropertyChanged(nameof(NamesByObjectId));
            }
        }

        private void RefreshNames() {
            if (!int.TryParse(this.Suffix, out int number)) {
                number = int.MinValue;
            }
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            for (int i = 0; i < NamesByObjectId.Count; i++) {
                string suffix;
                if (number != int.MinValue) {
                    suffix = (number + i).ToString().PadLeft(this.Suffix.Length, '0');
                }
                else {
                    suffix = this.Suffix + i.ToString();
                }
                list.Add(new Tuple<string, string>(NamesByObjectId[i].Item1, this.Prefix + suffix));
            }
            NamesByObjectId = list;
        }
    }
}
