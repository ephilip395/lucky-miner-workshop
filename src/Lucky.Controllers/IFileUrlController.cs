using Lucky.Core.MinerServer;

namespace Lucky.Controllers {
    public interface IFileUrlController : IPackageUrlController {
        string LuckyUrl(LuckyUrlRequest request);
        string LuckyUpdaterUrl();
        string MinerFinderUrl();
        string LiteDbExplorerUrl();
        string AtikmdagPatcherUrl();
        string SwitchRadeonGpuUrl();
        /// <summary>
        /// 基于文件码获取文件的下载地址，文件码必须是白名单中的成员。
        /// 白名单：LuckyUpdater,MinerFinder,LiteDbExplorer,AtikmdagPatcher,SwitchRadeonGpu
        /// </summary>
        /// <param name="fileCode">文件码</param>
        /// <returns></returns>
        string ToolFileUrl(string fileCode);
    }
}
