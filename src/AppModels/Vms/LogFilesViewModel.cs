﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Lucky.Vms {
    public class LogFilesViewModel : ViewModelBase {
        public class LogFile {
            public LogFile(string fileName, DateTime lastWriteTime, string fileFullName) {
                this.FileName = fileName;
                this.LastWriteTime = lastWriteTime;
                this.LastWriteTimeText = Timestamp.GetTimestampText(lastWriteTime);
                this.FileFullName = fileFullName;
            }

            public string FileFullName { get; private set; }
            public string FileName { get; private set; }
            public DateTime LastWriteTime { get; private set; }
            public string LastWriteTimeText { get; private set; }
        }
        private List<LogFile> _logFiles;
        private LogFile _selectedLogFile;
        public ICommand OpenLogFile { get; private set; }

        public LogFilesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.OpenLogFile = new DelegateCommandTpl<string>((fileFullName) => {
                AppRoot.FileOpener.Open(fileFullName);
            });
        }

        public List<LogFile> LogFiles {
            get => _logFiles;
            set {
                _logFiles = value;
                OnPropertyChanged(nameof(LogFiles));
            }
        }

        public LogFile SelectedLogFile {
            get => _selectedLogFile;
            set {
                _selectedLogFile = value;
                OnPropertyChanged(nameof(SelectedLogFile));
            }
        }

        public void RefreshLogFiles() {
            if (!Directory.Exists(Logger.DirFullPath)) {
                return;
            }
            this.LogFiles = Directory.GetFiles(Logger.DirFullPath).Select(a => {
                FileInfo fileInfo = new FileInfo(a);
                return new LogFile(fileInfo.Name, fileInfo.LastWriteTime, fileFullName: a);
            }).OrderByDescending(a => a.LastWriteTime).ToList();
            this.SelectedLogFile = this.LogFiles.FirstOrDefault(a => !a.FileName.StartsWith("root"));
        }

        public string GetLatestLogFileFullName() {
            if (!Directory.Exists(Logger.DirFullPath)) {
                return null;
            }
            string fileFullName = null;
            try {
                string latestOne = null;
                DateTime lastWriteTime = DateTime.MinValue;
                FileInfo fileInfo;
                foreach (var itemFullName in Directory.GetFiles(Logger.DirFullPath)) {
                    fileInfo = new FileInfo(itemFullName);
                    if (fileInfo.Name.StartsWith("root")) {
                        continue;
                    }
                    if (fileInfo.LastWriteTime > lastWriteTime) {
                        lastWriteTime = fileInfo.LastWriteTime;
                        latestOne = itemFullName;
                    }
                }
                fileFullName = latestOne;
            }
            catch {
            }
            return fileFullName;
        }
    }
}
