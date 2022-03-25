using Lucky.Core;
using Lucky.Core.Kernels;
using Lucky.Core.Profile;
using Lucky.Gpus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lucky.Mine {
    public static class MineContextFactory {
        private class ParameterNames {
            // 根据这个判断是否换成过期
            internal string Body = string.Empty;
            internal readonly HashSet<string> Names = new HashSet<string>();
        }

        public static IMineContext CreateMineContext() {
            ILuckyContext luckycmContext = LuckyContext.Instance;
            var minerProfile = luckycmContext.MinerProfile;
            var serverContext = luckycmContext.ServerContext;
            var gpuSet = luckycmContext.GpuSet;
            if (!luckycmContext.GetProfileData(out ICoin mainCoin, out ICoinProfile mainCoinProfile, out IPool mainCoinPool, out ICoinKernel mainCoinKernel, out IKernel kernel,
                out IKernelInput kernelInput, out IKernelOutput kernelOutput, out string _)) {
                return null;
            }
            if (!kernel.IsSupported(mainCoin)) {
                return null;
            }
            ICoinKernelProfile coinKernelProfile = minerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId);
            string poolKernelArgs = string.Empty;
            IPoolKernel poolKernel = serverContext.PoolKernelSet.AsEnumerable().FirstOrDefault(a => a.PoolId == mainCoinPool.GetId() && a.KernelId == kernel.GetId());
            if (poolKernel != null) {
                poolKernelArgs = poolKernel.Args;
            }
            string kernelArgs = kernelInput.Args;
            string coinKernelArgs = mainCoinKernel.Args;
            string customArgs = coinKernelProfile.CustomArgs ?? string.Empty;
            var parameters = new Dictionary<string, string>();
            var fileWriters = new Dictionary<Guid, string>();
            var fragments = new Dictionary<Guid, string>();
            parameters.Add(LuckyKeyword.MainCoinParameterName, mainCoin.Code);
            string userName = string.Empty;
            string password = LuckyKeyword.PasswordDefaultValue;
            string wallet = mainCoinProfile.Wallet;
            bool isTestWallet = false;
            bool isTestUserName = false;
            IPoolProfile poolProfile = minerProfile.GetPoolProfile(mainCoinPool.GetId());
            if (mainCoinPool.IsUserMode) {
                password = poolProfile.Password;
                if (string.IsNullOrEmpty(password)) {
                    password = LuckyKeyword.PasswordDefaultValue;
                }
                userName = poolProfile.UserName;
                wallet = poolProfile.UserName;
                isTestUserName = serverContext.PoolSet.TryGetPool(poolProfile.PoolId, out IPool pool) && userName == pool.UserName;
            }
            else {
                userName = wallet;
                isTestWallet = serverContext.CoinSet.TryGetCoin(mainCoinProfile.CoinId, out ICoin coin) && wallet == coin.TestWallet;
            }
            parameters.Add(LuckyKeyword.UserNameParameterName, userName);
            parameters.Add(LuckyKeyword.PasswordParameterName, password);
            parameters.Add(LuckyKeyword.WalletParameterName, wallet);
            string server = mainCoinPool.Server;
            if (!string.IsNullOrWhiteSpace(poolProfile.Server)) {
                server = poolProfile.Server;
            }
            parameters.Add(LuckyKeyword.HostParameterName, GetHost(server));
            parameters.Add(LuckyKeyword.PortParameterName, GetPort(server).ToString());
            parameters.Add(LuckyKeyword.PoolParameterName, server);
            string minerName = $"{mainCoinPool.MinerNamePrefix}{minerProfile.MinerName}{mainCoinPool.MinerNamePostfix}";
            parameters.Add(LuckyKeyword.WorkerParameterName, minerName);
            if (mainCoinKernel.IsSupportPool1 && !mainCoinPool.NoPool1) {
                parameters.Add(LuckyKeyword.Worker1ParameterName, minerName);
                if (serverContext.PoolSet.TryGetPool(mainCoinProfile.PoolId1, out IPool mainCoinPool1)) {
                    IPoolProfile poolProfile1 = minerProfile.GetPoolProfile(mainCoinPool1.GetId());
                    string server1 = mainCoinPool1.Server;
                    if (!string.IsNullOrWhiteSpace(poolProfile1.Server)) {
                        server1 = poolProfile1.Server;
                    }
                    parameters.Add(LuckyKeyword.Host1ParameterName, GetHost(server1));
                    parameters.Add(LuckyKeyword.Port1ParameterName, GetPort(server1).ToString());
                    parameters.Add(LuckyKeyword.Pool1ParameterName, server1);
                    if (mainCoinPool1.IsUserMode) {
                        string password1 = poolProfile1.Password;
                        if (string.IsNullOrEmpty(password1)) {
                            password1 = LuckyKeyword.PasswordDefaultValue;
                        }
                        parameters.Add(LuckyKeyword.UserName1ParameterName, poolProfile1.UserName);
                        parameters.Add(LuckyKeyword.Password1ParameterName, password1);
                    }
                    else {
                        parameters.Add(LuckyKeyword.Wallet1ParameterName, mainCoinProfile.Wallet);
                    }
                }
            }
            string devicesArgs = GetDevicesArgs(kernelInput);
            // 这里不要考虑{logfile}，{logfile}往后推迟
            if (coinKernelProfile.IsDualCoinEnabled && kernelInput.IsSupportDualMine) {
                Guid dualCoinGroupId = mainCoinKernel.DualCoinGroupId;
                if (dualCoinGroupId != Guid.Empty) {
                    if (serverContext.CoinSet.TryGetCoin(coinKernelProfile.DualCoinId, out ICoin dualCoin)) {
                        ICoinProfile dualCoinProfile = minerProfile.GetCoinProfile(dualCoin.GetId());
                        if (serverContext.PoolSet.TryGetPool(dualCoinProfile.DualCoinPoolId, out IPool dualCoinPool)) {
                            string dualUserName = string.Empty;
                            string dualPassword = LuckyKeyword.PasswordDefaultValue;
                            string dualWallet = dualCoinProfile.DualCoinWallet;
                            parameters.Add(LuckyKeyword.DualCoinParameterName, dualCoin.Code);
                            IPoolProfile dualPoolProfile = minerProfile.GetPoolProfile(dualCoinPool.GetId());
                            if (dualCoinPool.IsUserMode) {
                                dualPassword = dualPoolProfile.Password;
                                if (string.IsNullOrEmpty(dualPassword)) {
                                    dualPassword = LuckyKeyword.PasswordDefaultValue;
                                }
                                dualUserName = dualPoolProfile.UserName;
                                dualWallet = dualPoolProfile.UserName;
                            }
                            else {
                                dualUserName = dualWallet;
                            }
                            parameters.Add(LuckyKeyword.DualUserNameParameterName, dualUserName);
                            parameters.Add(LuckyKeyword.DualPasswordParameterName, dualPassword);
                            parameters.Add(LuckyKeyword.DualWalletParameterName, dualWallet);
                            string dualCoinPoolServer = dualCoinPool.Server;
                            if (!string.IsNullOrWhiteSpace(dualPoolProfile.Server)) {
                                dualCoinPoolServer = dualPoolProfile.Server;
                            }
                            parameters.Add(LuckyKeyword.DualHostParameterName, GetHost(dualCoinPoolServer));
                            parameters.Add(LuckyKeyword.DualPortParameterName, GetPort(dualCoinPoolServer).ToString());
                            parameters.Add(LuckyKeyword.DualPoolParameterName, dualCoinPoolServer);

                            kernelArgs = mainCoinKernel.DualFullArgs;
                            AssembleArgs(parameters, ref kernelArgs, isDual: true);
                            AssembleArgs(parameters, ref poolKernelArgs, isDual: true);
                            AssembleArgs(parameters, ref customArgs, isDual: true);

                            string dualWeightArg;
                            if (!string.IsNullOrEmpty(kernelInput.DualWeightArg)) {
                                if (coinKernelProfile.IsAutoDualWeight && kernelInput.IsAutoDualWeight) {
                                    dualWeightArg = string.Empty;
                                }
                                else {
                                    dualWeightArg = $"{kernelInput.DualWeightArg} {Convert.ToInt32(coinKernelProfile.DualCoinWeight).ToString()}";
                                }
                            }
                            else {
                                dualWeightArg = string.Empty;
                            }
                            StringBuilder dualSb = new StringBuilder();
                            dualSb.Append(kernelArgs);
                            if (!string.IsNullOrEmpty(dualWeightArg)) {
                                dualSb.Append(" ").Append(dualWeightArg);
                            }
                            if (!string.IsNullOrEmpty(poolKernelArgs)) {
                                dualSb.Append(" ").Append(poolKernelArgs);
                            }
                            BuildFragments(serverContext, mainCoinKernel, parameters, out fileWriters, out fragments);
                            foreach (var fragment in fragments.Values) {
                                dualSb.Append(" ").Append(fragment);
                            }
                            if (!string.IsNullOrEmpty(customArgs)) {
                                dualSb.Append(" ").Append(customArgs);
                            }
                            if (!string.IsNullOrEmpty(devicesArgs)) {
                                dualSb.Append(" ").Append(devicesArgs);
                            }

                            // 注意：这里退出
                            return new DualMineContext(
                                new MineContext(
                                    minerProfile.MinerName,
                                    mainCoin,
                                    mainCoinPool,
                                    kernel,
                                    kernelInput,
                                    kernelOutput,
                                    mainCoinKernel,
                                    wallet,
                                    dualSb.ToString(),
                                    isTestWallet,
                                    isTestUserName,
                                    parameters,
                                    fragments,
                                    fileWriters,
                                    gpuSet.GetUseDevices()),
                                dualCoin,
                                dualCoinPool,
                                dualWallet,
                                coinKernelProfile.DualCoinWeight,
                                parameters,
                                fragments,
                                fileWriters,
                                gpuSet.GetUseDevices());
                        }
                    }
                }
            }
            AssembleArgs(parameters, ref kernelArgs, isDual: false);
            AssembleArgs(parameters, ref coinKernelArgs, isDual: false);
            AssembleArgs(parameters, ref poolKernelArgs, isDual: false);
            AssembleArgs(parameters, ref customArgs, isDual: false);
            StringBuilder sb = new StringBuilder();
            sb.Append(kernelArgs);
            if (!string.IsNullOrEmpty(coinKernelArgs)) {
                sb.Append(" ").Append(coinKernelArgs);
            }
            if (!string.IsNullOrEmpty(poolKernelArgs)) {
                sb.Append(" ").Append(poolKernelArgs);
            }
            if (!string.IsNullOrEmpty(devicesArgs)) {
                sb.Append(" ").Append(devicesArgs);
            }
            BuildFragments(serverContext, mainCoinKernel, parameters, out fileWriters, out fragments);
            foreach (var fragment in fragments.Values) {
                sb.Append(" ").Append(fragment);
            }
            if (!string.IsNullOrEmpty(customArgs)) {
                sb.Append(" ").Append(customArgs);
            }

            return new MineContext(
                minerProfile.MinerName,
                mainCoin,
                mainCoinPool,
                kernel,
                kernelInput,
                kernelOutput,
                mainCoinKernel,
                wallet,
                sb.ToString(),
                isTestWallet,
                isTestUserName,
                parameters,
                fragments,
                fileWriters,
                gpuSet.GetUseDevices());
        }

        #region 私有方法
        private static string GetHost(string server) {
            if (string.IsNullOrEmpty(server)) {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(server)) {
                return string.Empty;
            }
            int index = server.IndexOf(':');
            if (index > 0) {
                return server.Substring(0, index);
            }
            return server;
        }

        private static int GetPort(string server) {
            if (string.IsNullOrEmpty(server)) {
                return 0;
            }
            if (string.IsNullOrEmpty(server)) {
                return 0;
            }
            int index = server.IndexOf(':');
            if (index > 0) {
                int port;
                int.TryParse(server.Substring(index + 1), out port);
                return port;
            }
            return 0;
        }

        private static void BuildFragments(IServerContext serverContext, ICoinKernel coinKernel, Dictionary<string, string> parameters, out Dictionary<Guid, string> fileWriters, out Dictionary<Guid, string> fragments) {
            fileWriters = new Dictionary<Guid, string>();
            fragments = new Dictionary<Guid, string>();
            try {
                if (coinKernel.FragmentWriterIds != null && coinKernel.FragmentWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FragmentWriterIds) {
                        if (serverContext.FragmentWriterSet.TryGetFragmentWriter(writerId, out IFragmentWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
                if (coinKernel.FileWriterIds != null && coinKernel.FileWriterIds.Count != 0) {
                    foreach (var writerId in coinKernel.FileWriterIds) {
                        if (serverContext.FileWriterSet.TryGetFileWriter(writerId, out IFileWriter writer)) {
                            BuildFragment(parameters, fileWriters, fragments, writer);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static string GetDevicesArgs(IKernelInput kernelInput) {
            ILuckyContext luckycmContext = LuckyContext.Instance;
            var gpuSet = luckycmContext.GpuSet;
            string devicesArgs = string.Empty;
            if (!string.IsNullOrWhiteSpace(kernelInput.DevicesArg)) {
                int[] useDevices = gpuSet.GetUseDevices();
                if ((useDevices.Length != 0 && useDevices.Length != gpuSet.Count) || kernelInput.IsDeviceAllNotEqualsNone) {
                    string separator = kernelInput.DevicesSeparator;
                    // 因为空格在界面上不易被人读取所以以关键字代替空格
                    if (kernelInput.DevicesSeparator == LuckyKeyword.SpaceKeyword) {
                        separator = " ";
                    }
                    List<string> gpuIndexes = new List<string>();
                    foreach (var index in useDevices) {
                        int i = index;
                        if (kernelInput.DeviceBaseIndex != 0) {
                            i = index + kernelInput.DeviceBaseIndex;
                        }
                        string nText = LuckyKeyword.GetIndexChar(i, separator);
                        gpuIndexes.Add(nText);
                    }
                    switch (gpuSet.GpuType) {
                        case GpuType.Empty:
                            break;
                        case GpuType.NVIDIA:
                            devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, gpuIndexes.Select(a => $"{kernelInput.NDevicePrefix}{a}{kernelInput.NDevicePostfix}"))}";
                            break;
                        case GpuType.AMD:
                            devicesArgs = $"{kernelInput.DevicesArg} {string.Join(separator, gpuIndexes.Select(a => $"{kernelInput.ADevicePrefix}{a}{kernelInput.ADevicePostfix}"))}";
                            break;
                        default:
                            break;
                    }
                }
            }
            return devicesArgs;
        }

        private static readonly HashSet<string> _mainParameterNames = new HashSet<string> {
            LuckyKeyword.MainCoinParameterName,
            LuckyKeyword.WalletParameterName,
            LuckyKeyword.UserNameParameterName,
            LuckyKeyword.PasswordParameterName,
            LuckyKeyword.HostParameterName,
            LuckyKeyword.PortParameterName,
            LuckyKeyword.PoolParameterName,
            LuckyKeyword.Pool1ParameterName,
            LuckyKeyword.WorkerParameterName
        };
        private static readonly HashSet<string> _dualParameterNames = new HashSet<string> {
            LuckyKeyword.DualCoinParameterName,
            LuckyKeyword.DualWalletParameterName,
            LuckyKeyword.DualUserNameParameterName,
            LuckyKeyword.DualPasswordParameterName,
            LuckyKeyword.DualHostParameterName,
            LuckyKeyword.DualPortParameterName,
            LuckyKeyword.DualPoolParameterName
        };
        private static void AssembleArgs(Dictionary<string, string> prms, ref string args, bool isDual) {
            if (string.IsNullOrEmpty(args)) {
                args = string.Empty;
                return;
            }
            foreach (var parameterName in _mainParameterNames) {
                if (prms.ContainsKey(parameterName)) {
                    args = args.Replace("{" + parameterName + "}", prms[parameterName]);
                }
            }
            if (isDual) {
                foreach (var parameterName in _dualParameterNames) {
                    if (prms.ContainsKey(parameterName)) {
                        args = args.Replace("{" + parameterName + "}", prms[parameterName]);
                    }
                }
            }
            // 防止命令行上还有备用矿池，将备用矿池替换为主矿池
            args = args.Replace("{" + LuckyKeyword.Pool1ParameterName + "}", prms[LuckyKeyword.PoolParameterName]);
            // 这里不要考虑{logfile}，{logfile}往后推迟
        }

        private static readonly object _locker = new object();
        private static readonly Dictionary<Guid, ParameterNames> _parameterNameDic = new Dictionary<Guid, ParameterNames>();
        private static readonly string logfile = LuckyKeyword.LogFileParameterName.TrimStart('{').TrimEnd('}');
        private static ParameterNames GetParameterNames(IFragmentWriter writer) {
            if (string.IsNullOrEmpty(writer.Body)) {
                return new ParameterNames {
                    Body = writer.Body
                };
            }
            Guid writerId = writer.GetId();
            if (_parameterNameDic.TryGetValue(writerId, out ParameterNames parameterNames) && parameterNames.Body == writer.Body) {
                return parameterNames;
            }
            else {
                lock (_locker) {
                    if (_parameterNameDic.TryGetValue(writerId, out parameterNames) && parameterNames.Body == writer.Body) {
                        return parameterNames;
                    }
                    if (parameterNames != null) {
                        parameterNames.Body = writer.Body;
                    }
                    else {
                        parameterNames = new ParameterNames {
                            Body = writer.Body
                        };
                        _parameterNameDic.Add(writerId, parameterNames);
                    }
                    parameterNames.Names.Clear();
                    const string pattern = @"\{(\w+)\}";
                    var matches = Regex.Matches(writer.Body, pattern);
                    foreach (Match match in matches) {
                        string v = match.Groups[1].Value;
                        // 因为logfile在开始挖矿后才有值
                        if (v != logfile) {
                            parameterNames.Names.Add(v);
                        }
                    }
                    return parameterNames;
                }
            }
        }

        private static bool IsMatch(IFragmentWriter writer, Dictionary<string, string> parameters, out ParameterNames parameterNames) {
            parameterNames = GetParameterNames(writer);
            if (string.IsNullOrEmpty(writer.Body)) {
                return false;
            }
            if (parameterNames.Names.Count == 0) {
                return true;
            }
            foreach (var name in parameterNames.Names) {
                if (!parameters.ContainsKey(name)) {
                    return false;
                }
            }
            return true;
        }

        private static void BuildFragment(Dictionary<string, string> parameters, Dictionary<Guid, string> fileWriters, Dictionary<Guid, string> fragments, IFragmentWriter writer) {
            try {
                if (!IsMatch(writer, parameters, out ParameterNames parameterNames)) {
                    return;
                }
                string content = writer.Body;
                foreach (var parameterName in parameterNames.Names) {
                    if (parameters.ContainsKey(parameterName)) {
                        content = content.Replace($"{{{parameterName}}}", parameters[parameterName]);
                    }
                }
                if (writer is IFileWriter) {
                    if (fileWriters.ContainsKey(writer.GetId())) {
                        fileWriters[writer.GetId()] = content;
                    }
                    else {
                        fileWriters.Add(writer.GetId(), content);
                    }
                }
                else {
                    if (fragments.ContainsKey(writer.GetId())) {
                        fragments[writer.GetId()] = content;
                    }
                    else {
                        fragments.Add(writer.GetId(), content);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion
    }
}
