using Lucky.Core.Kernels;
using System;

namespace Lucky.Core.Profiles
{
    public interface IKernelProfile
    {
        Guid KernelId { get; }
        InstallStatus InstallStatus { get; }
    }
}
