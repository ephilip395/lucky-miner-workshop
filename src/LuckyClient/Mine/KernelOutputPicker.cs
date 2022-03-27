﻿using Lucky.Core;
using Lucky.Gpus;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lucky.Mine
{
    public static class KernelOutputPicker
    {
        private static string _preline;
        public static void Pick(ref string line, IMineContext mineContext)
        {
            try
            {
                if (string.IsNullOrEmpty(line))
                {
                    return;
                }
                // 注意：硬编码逻辑。使用Claymore挖非ETH币种时它也打印ETH，所以这里需要纠正它
                if ("Claymore".Equals(mineContext.Kernel.Code, StringComparison.OrdinalIgnoreCase))
                {
                    if (mineContext.MainCoin.Code != "ETH" && line.Contains("ETH"))
                    {
                        line = line.Replace("ETH", mineContext.MainCoin.Code);
                    }
                }
                ICoin coin = mineContext.MainCoin;
                bool isDual = false;
                Guid poolId = mineContext.MainCoinPool.GetId();
                // 如果是双挖上下文且当前输入行中没有主币关键字则视为双挖币
                if ((mineContext is IDualMineContext dualMineContext) && !line.Contains(mineContext.MainCoin.Code))
                {
                    isDual = true;
                    coin = dualMineContext.DualCoin;
                    poolId = dualMineContext.DualCoinPool.GetId();
                }
                ILuckyContext luckycmContext = LuckyContext.Instance;
                // 这些方法输出的是事件消息
                #region 总
                PickTotalSpeed(luckycmContext, line, mineContext.KernelOutput, isDual);
                PickTotalShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                PickAcceptShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                PickRejectShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                PickRejectPercent(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                #endregion

                #region 一个
                if (!isDual)
                {
                    // 决定不支持双挖的单卡份额统计
                    PicFoundOneShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput);
                    PicGotOneIncorrectShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput);
                }
                PickAcceptOneShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                PickRejectOneShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                #endregion

                #region 单卡
                PickGpuSpeed(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuAcceptShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuRejectShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuIncorrectShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                #endregion
                PickPoolDelay(line, mineContext.KernelOutput, isDual, poolId);
                // 如果是像BMiner那样的主币和双挖币的输出在同一行那样的模式则一行输出既要视为主币又要视为双挖币
                if (isDual && mineContext.KernelOutput.IsDualInSameLine)
                {
                    coin = mineContext.MainCoin;
                    isDual = false;
                    #region 总
                    PickTotalSpeed(luckycmContext, line, mineContext.KernelOutput, isDual);
                    PickTotalShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                    PickAcceptShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                    PickRejectShare(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                    PickRejectPercent(luckycmContext, line, mineContext.KernelOutput, coin, isDual);
                    #endregion

                    #region 一个
                    PickAcceptOneShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                    PickRejectOneShare(luckycmContext, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                    #endregion

                    #region 单卡
                    PickGpuSpeed(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuAcceptShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuRejectShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuIncorrectShare(luckycmContext, mineContext, line, mineContext.KernelOutput, isDual);
                    #endregion
                    PickPoolDelay(line, mineContext.KernelOutput, isDual, poolId);
                }
                _preline = line;
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
        }

        #region private static methods
        #region 总
        #region PickTotalSpeed
        private static void PickTotalSpeed(ILuckyContext luckycmContext, string line, IKernelOutput kernelOutput, bool isDual)
        {
            string totalSpeedPattern = kernelOutput.TotalSpeedPattern;
            if (isDual)
            {
                totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
            }
            if (string.IsNullOrEmpty(totalSpeedPattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(totalSpeedPattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string totalSpeedText = match.Groups[LuckyKeyword.TotalSpeedGroupName].Value;
                string totalSpeedUnit = match.Groups[LuckyKeyword.TotalSpeedUnitGroupName].Value;
                if (string.IsNullOrEmpty(totalSpeedUnit))
                {
                    if (isDual)
                    {
                        totalSpeedUnit = kernelOutput.DualSpeedUnit;
                    }
                    else
                    {
                        totalSpeedUnit = kernelOutput.SpeedUnit;
                    }
                }
                if (double.TryParse(totalSpeedText, out double totalSpeed))
                {
                    totalSpeed = totalSpeed.FromUnitSpeed(totalSpeedUnit);
                    var now = DateTime.Now;
                    IGpusSpeed gpuSpeeds = LuckyContext.Instance.GpusSpeed;
                    gpuSpeeds.SetCurrentSpeed(LuckyContext.GpuAllId, totalSpeed, isDual, now);
                    string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
                    if (isDual)
                    {
                        gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
                    }
                    // 如果没有单卡算力正则则平分总算力作为单卡算力正则
                    if ((string.IsNullOrEmpty(gpuSpeedPattern) || luckycmContext.GpuSet.Count == 1) && luckycmContext.GpuSet.Count != 0)
                    {
                        double gpuSpeed = totalSpeed / luckycmContext.GpuSet.Count;
                        foreach (var item in gpuSpeeds.AsEnumerable())
                        {
                            if (item.Gpu.Index != LuckyContext.GpuAllId)
                            {
                                gpuSpeeds.SetCurrentSpeed(item.Gpu.Index, gpuSpeed, isDual, now);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PickTotalShare
        private static void PickTotalShare(ILuckyContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string totalSharePattern = kernelOutput.TotalSharePattern;
            if (isDual)
            {
                totalSharePattern = kernelOutput.DualTotalSharePattern;
            }
            if (string.IsNullOrEmpty(totalSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(totalSharePattern);
            var match = regex.Match(line);
            if (match.Success)
            {
                string totalShareText = match.Groups[LuckyKeyword.TotalShareGroupName].Value;
                if (int.TryParse(totalShareText, out int totalShare))
                {
                    ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: totalShare - share.RejectShareCount, rejectShareCount: null, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickAcceptShare
        private static void PickAcceptShare(ILuckyContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string acceptSharePattern = kernelOutput.AcceptSharePattern;
            if (isDual)
            {
                acceptSharePattern = kernelOutput.DualAcceptSharePattern;
            }
            if (string.IsNullOrEmpty(acceptSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(acceptSharePattern);
            var match = regex.Match(line);
            if (match.Success)
            {
                string acceptShareText = match.Groups[LuckyKeyword.AcceptShareGroupName].Value;
                if (int.TryParse(acceptShareText, out int acceptShare))
                {
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: acceptShare, rejectShareCount: null, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickRejectShare
        private static void PickRejectShare(ILuckyContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string rejectSharePattern = kernelOutput.RejectSharePattern;
            if (isDual)
            {
                rejectSharePattern = kernelOutput.DualRejectSharePattern;
            }
            if (string.IsNullOrEmpty(rejectSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(rejectSharePattern);
            var match = regex.Match(line);
            if (match.Success)
            {
                string rejectShareText = match.Groups[LuckyKeyword.RejectShareGroupName].Value;

                if (int.TryParse(rejectShareText, out int rejectShare))
                {
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: null, rejectShareCount: rejectShare, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickRejectPercent
        private static void PickRejectPercent(ILuckyContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string rejectPercentPattern = kernelOutput.RejectPercentPattern;
            if (isDual)
            {
                rejectPercentPattern = kernelOutput.DualRejectPercentPattern;
            }
            if (string.IsNullOrEmpty(rejectPercentPattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(rejectPercentPattern);
            var match = regex.Match(line);
            string rejectPercentText = match.Groups[LuckyKeyword.RejectPercentGroupName].Value;
            if (double.TryParse(rejectPercentText, out double rejectPercent))
            {
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: null, rejectShareCount: (int)(share.TotalShareCount * rejectPercent), now: DateTime.Now);
            }
        }
        #endregion
        #endregion

        #region 一个
        #region PicFoundOneShare
        private static void PicFoundOneShare(ILuckyContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput)
        {
            string foundOneShare = kernelOutput.FoundOneShare;
            if (string.IsNullOrEmpty(foundOneShare))
            {
                return;
            }
            if (foundOneShare.Contains("\n"))
            {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(foundOneShare);
            var match = regex.Match(line);
            if (match.Success)
            {
                string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText))
                {
                    if (int.TryParse(gpuText, out int gpuIndex))
                    {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                        {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                    }
                }
            }
        }
        #endregion

        #region PicGotOneIncorrectShare
        private static void PicGotOneIncorrectShare(ILuckyContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput)
        {
            string pattern = kernelOutput.GpuGotOneIncorrectShare;
            if (string.IsNullOrEmpty(pattern))
            {
                return;
            }
            if (pattern.Contains("\n"))
            {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(pattern);
            var match = regex.Match(line);
            if (match.Success)
            {
                string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText))
                {
                    if (int.TryParse(gpuText, out int gpuIndex))
                    {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                        {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        context.GpusSpeed.IncreaseIncorrectShare(gpuIndex);
                    }
                }
            }
        }
        #endregion

        #region PickAcceptOneShare
        private static void PickAcceptOneShare(ILuckyContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string acceptOneShare = kernelOutput.AcceptOneShare;
            if (isDual)
            {
                acceptOneShare = kernelOutput.DualAcceptOneShare;
            }
            if (string.IsNullOrEmpty(acceptOneShare))
            {
                return;
            }
            if (acceptOneShare.Contains("\n"))
            {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(acceptOneShare);
            var match = regex.Match(line);
            if (match.Success)
            {
                if (!isDual)
                {
                    // 决定不支持双挖的单卡份额统计
                    string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                    if (!string.IsNullOrEmpty(gpuText))
                    {
                        if (int.TryParse(gpuText, out int gpuIndex))
                        {
                            if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                            {
                                gpuIndex = mineContext.UseDevices[gpuIndex];
                            }
                            if (string.IsNullOrEmpty(kernelOutput.FoundOneShare))
                            {
                                context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                            }
                            context.GpusSpeed.IncreaseAcceptShare(gpuIndex);
                        }
                    }
                }
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: share.AcceptShareCount + 1, rejectShareCount: null, now: DateTime.Now);
            }
        }
        #endregion

        #region PickRejectOneShare
        private static void PickRejectOneShare(ILuckyContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput, ICoin coin, bool isDual)
        {
            string rejectOneShare = kernelOutput.RejectOneShare;
            if (isDual)
            {
                rejectOneShare = kernelOutput.DualRejectOneShare;
            }
            if (string.IsNullOrEmpty(rejectOneShare))
            {
                return;
            }
            if (rejectOneShare.Contains("\n"))
            {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(rejectOneShare);
            var match = regex.Match(line);
            if (match.Success)
            {
                if (!isDual)
                {
                    // 决定不支持双挖的单卡份额统计
                    string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                    if (!string.IsNullOrEmpty(gpuText))
                    {
                        if (int.TryParse(gpuText, out int gpuIndex))
                        {
                            if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                            {
                                gpuIndex = mineContext.UseDevices[gpuIndex];
                            }
                            if (string.IsNullOrEmpty(kernelOutput.FoundOneShare))
                            {
                                context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                            }
                            context.GpusSpeed.IncreaseRejectShare(gpuIndex);
                        }
                    }
                    else if (!string.IsNullOrEmpty(kernelOutput.FoundOneShare))
                    {
                        // 哪个GPU最近找到了一个share就是那个GPU拒绝了一个share
                        var gpuSpeeds = context.GpusSpeed.AsEnumerable();
                        IGpuSpeed gpuSpeed = null;
                        foreach (var item in gpuSpeeds)
                        {
                            if (gpuSpeed == null)
                            {
                                gpuSpeed = item;
                            }
                            else if (item.FoundShareOn > gpuSpeed.FoundShareOn)
                            {
                                gpuSpeed = item;
                            }
                        }
                        if (gpuSpeed != null)
                        {
                            var gpuIndex = gpuSpeed.Gpu.Index;
                            context.GpusSpeed.IncreaseRejectShare(gpuIndex);
                        }
                    }
                }
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), null, share.RejectShareCount + 1, DateTime.Now);
            }
        }
        #endregion
        #endregion

        #region 单卡
        #region PickGpuSpeed
        private static void PickGpuSpeed(ILuckyContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual)
        {
            string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
            if (isDual)
            {
                gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
            }
            if (string.IsNullOrEmpty(gpuSpeedPattern))
            {
                return;
            }
            var now = DateTime.Now;
            bool hasGpuId = gpuSpeedPattern.Contains($"?<{LuckyKeyword.GpuIndexGroupName}>");
            Regex regex = VirtualRoot.GetRegex(gpuSpeedPattern);
            MatchCollection matches = regex.Matches(line);
            if (matches.Count > 0)
            {
                IGpusSpeed gpuSpeeds = LuckyContext.Instance.GpusSpeed;
                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    string gpuSpeedText = match.Groups[LuckyKeyword.GpuSpeedGroupName].Value;
                    string gpuSpeedUnit = match.Groups[LuckyKeyword.GpuSpeedUnitGroupName].Value;
                    if (string.IsNullOrEmpty(gpuSpeedUnit))
                    {
                        if (isDual)
                        {
                            gpuSpeedUnit = kernelOutput.DualSpeedUnit;
                        }
                        else
                        {
                            gpuSpeedUnit = kernelOutput.SpeedUnit;
                        }
                    }
                    int gpuIndex = i;
                    if (hasGpuId)
                    {
                        string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                        if (!int.TryParse(gpuText, out gpuIndex))
                        {
                            gpuIndex = i;
                        }
                        else
                        {
                            gpuIndex -= kernelOutput.GpuBaseIndex;
                            if (gpuIndex < 0)
                            {
                                continue;
                            }
                        }
                    }
                    if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                    {
                        gpuIndex = mineContext.UseDevices[gpuIndex];
                    }
                    if (double.TryParse(gpuSpeedText, out double gpuSpeed))
                    {
                        double gpuSpeedL = gpuSpeed.FromUnitSpeed(gpuSpeedUnit);
                        gpuSpeeds.SetCurrentSpeed(gpuIndex, gpuSpeedL, isDual, now);
                    }
                }
                string totalSpeedPattern = kernelOutput.TotalSpeedPattern;
                if (isDual)
                {
                    totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
                }
                // 求和分算力，因为有些内核在只有一张卡时不输出总算力
                double speed = isDual ? gpuSpeeds.AsEnumerable().Where(a => a.Gpu.Index != LuckyContext.GpuAllId).Sum(a => a.DualCoinSpeed.Value)
                                     : gpuSpeeds.AsEnumerable().Where(a => a.Gpu.Index != LuckyContext.GpuAllId).Sum(a => a.MainCoinSpeed.Value);
                if (speed > (isDual ? gpuSpeeds.CurrentSpeed(LuckyContext.GpuAllId).DualCoinSpeed : gpuSpeeds.CurrentSpeed(LuckyContext.GpuAllId).MainCoinSpeed).Value)
                {
                    gpuSpeeds.SetCurrentSpeed(LuckyContext.GpuAllId, speed, isDual, now);
                }
            }
        }
        #endregion

        #region PicGpuAcceptShare
        private static void PicGpuAcceptShare(ILuckyContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual)
        {
            string gpuAcceptSharePattern = kernelOutput.GpuAcceptShare;
            if (isDual)
            {
                return;
            }
            if (string.IsNullOrEmpty(gpuAcceptSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuAcceptSharePattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                string acceptShareText = match.Groups[LuckyKeyword.AcceptShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText))
                {
                    if (int.TryParse(gpuText, out int gpuIndex))
                    {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                        {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(acceptShareText))
                        {
                            if (int.TryParse(acceptShareText, out int acceptShare))
                            {
                                context.GpusSpeed.SetAcceptShare(gpuIndex, acceptShare);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PicGpuRejectShare
        private static void PicGpuRejectShare(ILuckyContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual)
        {
            string gpuRejectSharePattern = kernelOutput.GpuRejectShare;
            if (isDual)
            {
                return;
            }
            if (string.IsNullOrEmpty(gpuRejectSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuRejectSharePattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                string rejectShareText = match.Groups[LuckyKeyword.RejectShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText))
                {
                    if (int.TryParse(gpuText, out int gpuIndex))
                    {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                        {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(rejectShareText))
                        {
                            if (int.TryParse(rejectShareText, out int rejectShare))
                            {
                                context.GpusSpeed.SetRejectShare(gpuIndex, rejectShare);
                                // TODO:如果gpuTotal的拒绝份额为0，求和拒绝份额
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PicGpuIncorrectShare
        private static void PicGpuIncorrectShare(ILuckyContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual)
        {
            string gpuIncorrectSharePattern = kernelOutput.GpuIncorrectShare;
            if (isDual)
            {
                return;
            }
            if (string.IsNullOrEmpty(gpuIncorrectSharePattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuIncorrectSharePattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string gpuText = match.Groups[LuckyKeyword.GpuIndexGroupName].Value;
                string incorrectShareText = match.Groups[LuckyKeyword.IncorrectShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText))
                {
                    if (int.TryParse(gpuText, out int gpuIndex))
                    {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length)
                        {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(incorrectShareText))
                        {
                            if (int.TryParse(incorrectShareText, out int incorrectShare))
                            {
                                context.GpusSpeed.SetIncorrectShare(gpuIndex, incorrectShare);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region PickPoolDelay
        private static void PickPoolDelay(string line, IKernelOutput kernelOutput, bool isDual, Guid poolId)
        {
            string poolDelayPattern = kernelOutput.PoolDelayPattern;
            if (isDual)
            {
                poolDelayPattern = kernelOutput.DualPoolDelayPattern;
            }
            if (string.IsNullOrEmpty(poolDelayPattern))
            {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(poolDelayPattern);
            Match match = regex.Match(line);
            if (match.Success)
            {
                string poolDelayText = match.Groups[LuckyKeyword.PoolDelayGroupName].Value;
                VirtualRoot.RaiseEvent(new PoolDelayPickedEvent(poolId, isDual, poolDelayText));
            }
        }
        #endregion

        private static bool IsMapGpuIndex(ILuckyContext context, IMineContext mineContext, IKernelOutput kernelOutput)
        {
            return kernelOutput.IsMapGpuIndex && !string.IsNullOrWhiteSpace(mineContext.KernelInput.DevicesArg)
                && mineContext.UseDevices.Length != 0 && mineContext.UseDevices.Length != context.GpuSet.Count;
        }
        #endregion
    }
}
