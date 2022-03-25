﻿using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Lucky.Vms {
    public class BrandTagViewModel : ViewModelBase {
        public ICommand TagKernelBrand { get; private set; }
        public ICommand TagPoolBrand { get; private set; }

        public BrandTagViewModel() {
            this.TagKernelBrand = new DelegateCommandTpl<SysDicItemViewModel>(brandItem => {
                string outFileName = Path.GetFileNameWithoutExtension(VirtualRoot.AppFileFullName) + $"_{brandItem.Value}.exe";
                string outDir = Path.GetDirectoryName(VirtualRoot.AppFileFullName);
                string outFileFullName = Path.Combine(outDir, outFileName);
                VirtualRoot.TagBrandId(LuckyKeyword.KernelBrandId, brandItem.GetId(), VirtualRoot.AppFileFullName, outFileFullName);
                VirtualRoot.Out.ShowSuccess($"打码成功:{outFileName}");
                Process.Start(outDir);
            }, brandItem => brandItem != SysDicItemViewModel.PleaseSelect);
            this.TagPoolBrand = new DelegateCommandTpl<SysDicItemViewModel>(brandItem => {
                string outFileName = Path.GetFileNameWithoutExtension(VirtualRoot.AppFileFullName) + $"_{brandItem.Value}.exe";
                string outDir = Path.GetDirectoryName(VirtualRoot.AppFileFullName);
                string outFileFullName = Path.Combine(outDir, outFileName);
                VirtualRoot.TagBrandId(LuckyKeyword.PoolBrandId, brandItem.GetId(), VirtualRoot.AppFileFullName, outFileFullName);
                VirtualRoot.Out.ShowSuccess($"打码成功:{outFileName}");
                Process.Start(outDir);
            }, brandItem => brandItem != SysDicItemViewModel.PleaseSelect);
        }

        public AppRoot.SysDicItemViewModels SysDicItemVms {
            get {
                return AppRoot.SysDicItemVms;
            }
        }
    }
}
