using Lucky.Core.MinerServer;

namespace Lucky.Controllers {
    public interface IPackageUrlController {
        string PackageUrl(PackageUrlRequest request);
    }
}
