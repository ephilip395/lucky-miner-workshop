using Lucky.Core;
using Lucky.Vms;
using System;
using System.IO;
using System.Windows.Input;

namespace Lucky.MinerMonitor.Vms {
    public class MinerFinderConfigViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        public ICommand Save { get; private set; }

        public MinerFinderConfigViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.FileName)) {
                        this.FileName = LuckyKeyword.MinerFinderFileName;
                    }
                    RpcRoot.OfficialServer.AppSettingService.SetAppSettingAsync(new AppSettingData {
                        Key = LuckyKeyword.MinerFinderFileNameAppSettingKey,
                        Value = this.FileName
                    }, (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
            RpcRoot.OfficialServer.FileUrlService.GetMinerFinderUrlAsync((fileDownloadUrl, e) => {
                if (!string.IsNullOrEmpty(fileDownloadUrl)) {
                    Uri uri = new Uri(fileDownloadUrl);
                    FileName = Path.GetFileName(uri.LocalPath);
                }
                else {
                    FileName = LuckyKeyword.MinerFinderFileName;
                }
            });
        }

        private string _fileName;
        public string FileName {
            get {
                return _fileName;
            }
            set {
                if (_fileName != value) {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }
    }
}
