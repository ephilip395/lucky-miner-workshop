﻿using Lucky.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms {
    public class ServerMessagesViewModel : ViewModelBase {
        private ObservableCollection<ServerMessageViewModel> _serverMessageVms;
        private ObservableCollection<ServerMessageViewModel> _queyResults;
        private string _keyword;
        private readonly Dictionary<ServerMessageType, MessageTypeItem<ServerMessageType>> _count = new Dictionary<ServerMessageType, MessageTypeItem<ServerMessageType>>();

        public ICommand Add { get; private set; }

        public ICommand ClearKeyword { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand ViewHistory { get; private set; }

        public ServerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            foreach (var messageType in Enums.ServerMessageTypeEnumItems) {
                _count.Add(messageType.Value, new MessageTypeItem<ServerMessageType>(messageType, ServerMessageViewModel.GetIcon, ServerMessageViewModel.GetIconFill, RefreshQueryResults));
            }
            Init();
            this.Add = new DelegateCommand(() => {
                new ServerMessageViewModel(new ServerMessageData {
                    Id = Guid.NewGuid(),
                    MessageType = nameof(ServerMessageType.Info),
                    Provider = "admin",
                    Content = string.Empty,
                    Timestamp = DateTime.MinValue
                }).Edit.Execute(FormType.Add);
            });
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
            this.Clear = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: "确定清空吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new ClearServerMessagesCommand());
                }));
            });
            this.ViewHistory = new DelegateCommand(() => {
                this.ShowSoftDialog(new DialogWindowViewModel(message: "确定显示历史消息吗？", title: "确认", onYes: () => {
                    VirtualRoot.LocalServerMessageSetTimestamp = Timestamp.UnixBaseTime;
                    VirtualRoot.Execute(new LoadNewServerMessageCommand());
                }));
            });
            VirtualRoot.BuildEventPath<ServerMessagesClearedEvent>("清空了本地存储的服务器消息后刷新Vm内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    Init();
                });
            VirtualRoot.BuildEventPath<NewServerMessageLoadedEvent>("从服务器加载了新消息后刷新Vm内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    UIThread.Execute(() => {
                        foreach (var item in message.Data) {
                            var vm = new ServerMessageViewModel(item);
                            var exist = _serverMessageVms.FirstOrDefault(a => a.Id == item.Id);
                            if (exist != null) {
                                _serverMessageVms.Remove(exist);
                                if (item.IsDeleted) {
                                    _count[exist.MessageTypeEnum].Count--;
                                }
                                else {
                                    _serverMessageVms.Insert(0, vm);
                                    if (exist.MessageType != item.MessageType) {
                                        _count[exist.MessageTypeEnum].Count--;
                                        _count[vm.MessageTypeEnum].Count++;
                                    }
                                }
                            }
                            else if (!vm.IsDeleted) {
                                _serverMessageVms.Insert(0, vm);
                                _count[vm.MessageTypeEnum].Count++;
                            }
                            if (IsSatisfyQuery(vm)) {
                                exist = _queyResults.FirstOrDefault(a => a.Id == item.Id);
                                if (exist != null) {
                                    _queyResults.Remove(exist);
                                }
                                if (!vm.IsDeleted) {
                                    _queyResults.Insert(0, vm);
                                }
                            }
                        }
                        OnPropertyChanged(nameof(IsNoRecord));
                    });
                });
            VirtualRoot.BuildEventPath<NewDayEvent>("新的一天到来时刷新消息集中的可读性时间戳展示", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                path: message => {
                    if (QueryResults == null) {
                        return;
                    }
                    foreach (var item in QueryResults) {
                        if (item.Timestamp.Date.AddDays(3) >= message.BornOn.Date) {
                            item.OnPropertyChanged(nameof(item.TimestampText));
                        }
                        else {
                            // 因为是按照时间倒叙排列的，所以可以break
                            break;
                        }
                    }
                });
        }

        private void Init() {
            var data = LuckyContext.Instance.ServerMessageSet.AsEnumerable().Where(a => !a.IsDeleted).Select(a => new ServerMessageViewModel(a));
            _serverMessageVms = new ObservableCollection<ServerMessageViewModel>(data);
            foreach (var key in _count.Keys) {
                _count[key].Count = 0;
            }
            foreach (var item in _serverMessageVms) {
                _count[item.MessageTypeEnum].Count++;
            }
            RefreshQueryResults();
        }

        public MainMenuViewModel MainMenu {
            get {
                return MainMenuViewModel.Instance;
            }
        }

        public IEnumerable<MessageTypeItem<ServerMessageType>> MessageTypeItems {
            get {
                return _count.Values;
            }
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    RefreshQueryResults();
                }
            }
        }

        public ObservableCollection<ServerMessageViewModel> QueryResults {
            get {
                return _queyResults;
            }
        }

        private bool IsSatisfyQuery(ServerMessageViewModel vm) {
            if (_queyResults == _serverMessageVms) {
                return false;
            }
            if (_count[vm.MessageTypeEnum].IsChecked && (string.IsNullOrEmpty(Keyword) || vm.Content.IgnoreCaseContains(Keyword))) {
                return true;
            }
            return false;
        }

        public bool IsNoRecord {
            get {
                return _queyResults.Count == 0;
            }
        }

        private void RefreshQueryResults() {
            bool isCheckedAllMessageType = _count.Values.All(a => a.IsChecked);
            if (isCheckedAllMessageType && string.IsNullOrEmpty(Keyword)) {
                if (_queyResults != _serverMessageVms) {
                    _queyResults = _serverMessageVms;
                    OnPropertyChanged(nameof(IsNoRecord));
                    OnPropertyChanged(nameof(QueryResults));
                }
                return;
            }
            var query = _serverMessageVms.AsQueryable();
            if (!isCheckedAllMessageType) {
                query = query.Where(a => _count[a.MessageTypeEnum].IsChecked);
            }
            if (!string.IsNullOrEmpty(Keyword)) {
                query = query.Where(a => a.Content != null && a.Content.IgnoreCaseContains(Keyword));
            }
            _queyResults = new ObservableCollection<ServerMessageViewModel>(query);
            OnPropertyChanged(nameof(IsNoRecord));
            OnPropertyChanged(nameof(QueryResults));
        }
    }
}
