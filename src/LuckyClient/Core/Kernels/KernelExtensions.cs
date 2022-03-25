﻿using Lucky.Gpus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lucky.Core.Kernels {
    public static class KernelExtensions {
        public sealed class CommandName {
            internal string Name;
            // 根据这个判断是否换成过期
            internal string KernelInputArgs;
        }

        public static string GetProcessName(this IKernel kernel) {
            string commandName = GetCommandName(kernel);
            if (string.IsNullOrEmpty(commandName)) {
                return string.Empty;
            }
            return Path.GetFileNameWithoutExtension(commandName);
        }

        private static readonly Dictionary<Guid, CommandName> _commandNames = new Dictionary<Guid, CommandName>();
        public static string GetCommandName(this IKernel kernel) {
            try {
                if (kernel == null || kernel.KernelInputId == Guid.Empty) {
                    return string.Empty;
                }
                LuckyContext.Instance.ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput);
                if (kernelInput == null) {
                    LuckyConsole.UserError("意外！没有正确配置内核输入，请联系客服解决。");
                    return string.Empty;
                }
                if (_commandNames.TryGetValue(kernel.GetId(), out CommandName commandName)) {
                    // 如果KernelInput.Args没有变那么命令名就没有变
                    if (kernelInput.Args == commandName.KernelInputArgs) {
                        return commandName.Name;
                    }
                }
                string args = kernelInput.Args;
                if (!string.IsNullOrEmpty(args)) {
                    args = args.Trim();
                }
                else {
                    return string.Empty;
                }
                string cmdName;
                if (args[0] == '"') {
                    int index = args.IndexOf('"', 1);
                    cmdName = args.Substring(1, index - 1);
                }
                else {
                    int firstSpaceIndex = args.IndexOf(' ');
                    if (firstSpaceIndex != -1) {
                        cmdName = args.Substring(0, args.IndexOf(' '));
                    }
                    else {
                        cmdName = args;
                    }
                }
                if (commandName != null) {
                    commandName.Name = cmdName;
                    commandName.KernelInputArgs = kernelInput.Args;
                }
                else {
                    _commandNames.Add(kernel.GetId(), new CommandName {
                        Name = cmdName,
                        KernelInputArgs = kernelInput.Args
                    });
                }
                return cmdName;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        public static bool IsSupported(this IKernel kernel, ICoin coin) {
            // 群控端和无显卡的电脑的GpuSet类型都是空
            if (LuckyContext.Instance.GpuSet.GpuType.IsEmpty()) {
                return true;
            }
            var coinKernelSet = LuckyContext.Instance.ServerContext.CoinKernelSet;
            foreach (var item in coinKernelSet.AsEnumerable().Where(a => a.CoinId == coin.GetId() && a.KernelId == kernel.GetId()).ToArray()) {
                if (item.SupportedGpu.IsSupportedGpu(LuckyContext.Instance.GpuSet.GpuType)) {
                    return true;
                }
            }
            return false;
        }

        public static string GetKernelDirFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(MinerTweakTempPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
        }

        public static string GetPackageFileFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(HomePath.PackagesDirFullName, kernel.Package);
        }

        public static bool IsPackageFileExist(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return false;
            }
            string fileFullName = kernel.GetPackageFileFullName();
            return File.Exists(fileFullName);
        }

        public static string GetDownloadFileFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(MinerTweakTempPath.DownloadDirFullName, kernel.Package);
        }

        public static bool ExtractPackage(this IKernel kernel) {
            try {
                string kernelDir = kernel.GetKernelDirFullName();
                if (string.IsNullOrEmpty(kernelDir)) {
                    return false;
                }
                if (!Directory.Exists(kernelDir)) {
                    Directory.CreateDirectory(kernelDir);
                }
                string packageZipFileFullName = kernel.GetPackageFileFullName();
                if (string.IsNullOrEmpty(packageZipFileFullName)) {
                    return false;
                }
                if (!File.Exists(packageZipFileFullName)) {
                    LuckyConsole.DevDebug(() => $"试图解压的{packageZipFileFullName}文件不存在");
                    return false;
                }
                ZipUtil.DecompressZipFile(packageZipFileFullName, kernelDir);
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }
    }
}
