﻿using Lucky.Core;
using System;
using System.Windows.Input;

namespace Lucky.Vms {
    public class FileWriterViewModel : ViewModelBase, IEditableViewModel, IFileWriter {
        private string _fileUrl;
        private Guid _id;
        private string _name;
        private string _body;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public FileWriterViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public FileWriterViewModel(IFileWriter data) : this(data.GetId()) {
            _name = data.Name;
            _fileUrl = data.FileUrl;
            _body = data.Body;
        }

        public FileWriterViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (LuckyContext.Instance.ServerContext.FileWriterSet.TryGetFileWriter(this.Id, out IFileWriter writer)) {
                    VirtualRoot.Execute(new UpdateFileWriterCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddFileWriterCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommandTpl<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditFileWriterCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}组吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveFileWriterCommand(this.Id));
                }));
            });
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string FileUrl {
            get => _fileUrl;
            set {
                _fileUrl = value;
                OnPropertyChanged(nameof(FileUrl));
            }
        }

        public string Body {
            get => _body;
            set {
                _body = value;
                OnPropertyChanged(nameof(Body));
            }
        }
    }
}
