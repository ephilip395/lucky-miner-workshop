﻿using Lucky.Core.MinerServer;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace Lucky.Vms {
    public class ServerMessageViewModel : ViewModelBase, IServerMessage, IEditableViewModel {
        private static readonly StreamGeometry InfoIcon = AppUtil.GetResource<StreamGeometry>("Icon_Message");
        private static readonly SolidColorBrush InfoColor = AppUtil.GetResource<SolidColorBrush>("InfoColor");
        private static readonly StreamGeometry NewVersionIcon = AppUtil.GetResource<StreamGeometry>("Icon_NewVersion");
        private static readonly SolidColorBrush NewVersionColor = AppUtil.GetResource<SolidColorBrush>("NewVersionColor");

        public static StreamGeometry GetIcon(ServerMessageType messageType) {
            switch (messageType) {
                case ServerMessageType.Info:
                    return InfoIcon;
                case ServerMessageType.NewVersion:
                    return NewVersionIcon;
                default:
                    return null;
            }
        }

        public static SolidColorBrush GetIconFill(ServerMessageType messageType) {
            switch (messageType) {
                case ServerMessageType.Info:
                    return InfoColor;
                case ServerMessageType.NewVersion:
                    return NewVersionColor;
                default:
                    return WpfUtil.BlackBrush;
            }
        }

        private Guid _id;
        private string _provider;
        private string _messageType;
        private string _content;
        private DateTime _timestamp;
        private bool _isDeleted;
        private ServerMessageType _messageTypeEnum;

        public EnumItem<ServerMessageType> ServerMessageTypeEnumItem {
            get {
                return _messageTypeEnum.GetEnumItem();
            }
            set {
                _messageTypeEnum = value.Value;
                _messageType = _messageTypeEnum.GetName();
                OnPropertyChanged(nameof(MessageType));
                OnPropertyChanged(nameof(ServerMessageTypeEnumItem));
                OnPropertyChanged(nameof(MessageTypeText));
                OnPropertyChanged(nameof(MessageTypeIcon));
                OnPropertyChanged(nameof(IconFill));
            }
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: LuckyKeyword.WpfDesignOnly, error: true)]
        public ServerMessageViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(LuckyKeyword.WpfDesignOnly);
            }
        }

        public ServerMessageViewModel(Guid id) {
            _id = id;
            this.Edit = new DelegateCommandTpl<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditServerMessageCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定标记删除'{this.Content}'这条消息吗？", title: "确认", onYes: () => {
                    MinerMonitor.MinerMonitorRoot.Login(() => {
                        VirtualRoot.Execute(new MarkDeleteServerMessageCommand(this.Id));
                    });
                }));
            });
            this.Save = new DelegateCommand(() => {
                MinerMonitor.MinerMonitorRoot.Login(() => {
                    VirtualRoot.Execute(new AddOrUpdateServerMessageCommand(this));
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                });
            });
        }

        public ServerMessageViewModel(IServerMessage data) : this(data.Id) {
            _provider = data.Provider;
            _messageType = data.MessageType;
            _content = data.Content;
            _timestamp = data.Timestamp;
            _isDeleted = data.IsDeleted;
            data.MessageType.TryParse(out _messageTypeEnum);
        }

        public ServerMessageType MessageTypeEnum {
            get {
                return _messageTypeEnum;
            }
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

        public string Provider {
            get => _provider;
            set {
                _provider = value;
                OnPropertyChanged(nameof(Provider));
            }
        }

        public string MessageType {
            get => _messageType;
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
                value.TryParse(out _messageTypeEnum);
                OnPropertyChanged(nameof(MessageTypeIcon));
                OnPropertyChanged(nameof(IconFill));
                OnPropertyChanged(nameof(MessageTypeText));
            }
        }

        public string Content {
            get => _content;
            set {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string MessageTypeText {
            get {
                return _messageTypeEnum.GetDescription();
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                return GetIcon(_messageTypeEnum);
            }
        }

        public SolidColorBrush IconFill {
            get {
                return GetIconFill(_messageTypeEnum);
            }
        }

        public DateTime Timestamp {
            get => _timestamp;
            set {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }

        public string TimestampText {
            get {
                return Lucky.Timestamp.GetTimestampText(this.Timestamp);
            }
        }

        public bool IsDeleted {
            get {
                return _isDeleted;
            }
            set {
                _isDeleted = value;
                OnPropertyChanged(nameof(IsDeleted));
            }
        }

        public MainMenuViewModel MainMenu {
            get {
                return MainMenuViewModel.Instance;
            }
        }
    }
}
