﻿using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lucky.Vms {
    public class FileWriterSelectViewModel : ViewModelBase {
        private FileWriterViewModel _selectedResult;
        public readonly Action<FileWriterViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public FileWriterSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public FileWriterSelectViewModel(Action<FileWriterViewModel> onOk) {
            OnOk = onOk;
        }

        public FileWriterViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<FileWriterViewModel> FileWriterVms {
            get {
                return AppRoot.FileWriterVms.List;
            }
        }
    }
}
