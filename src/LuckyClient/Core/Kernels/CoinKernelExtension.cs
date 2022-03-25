using System;

namespace Lucky.Core.Kernels {
    public static class CoinKernelExtension {
        public static bool GetIsSupportDualMine(this ICoinKernel coinKernel) {
            if (!LuckyContext.Instance.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out IKernel kernel)) {
                return false;
            }
            if (!LuckyContext.Instance.ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out IKernelInput kernelInput)) {
                return false;
            }
            if (!kernelInput.IsSupportDualMine) {
                return false;
            }
            return coinKernel.DualCoinGroupId != Guid.Empty && LuckyContext.Instance.ServerContext.GroupSet.TryGetGroup(coinKernel.DualCoinGroupId, out IGroup _);
        }
    }
}
