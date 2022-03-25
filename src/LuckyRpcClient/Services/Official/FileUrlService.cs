using Lucky.Controllers;
using Lucky.Core.MinerServer;
using System;

namespace Lucky.Services.Official
{
    public class FileUrlService
    {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IFileUrlController>();

        internal FileUrlService()
        {
        }

        #region GetLuckyUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetLuckyUrlAsync(string fileName, Action<string, Exception> callback)
        {
            LuckyUrlRequest request = new LuckyUrlRequest
            {
                FileName = fileName
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.LuckyUrl),
                request,
                callback);
        }
        #endregion

        #region GetLiteDbExplorerUrlAsync
        public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.LiteDbExplorerUrl),
                callback);
        }
        #endregion

        #region GetAtikmdagPatcherUrlAsync
        public void GetAtikmdagPatcherUrlAsync(Action<string, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.AtikmdagPatcherUrl),
                callback);
        }
        #endregion

        #region GetSwitchRadeonGpuUrlAsync
        public void GetSwitchRadeonGpuUrlAsync(Action<string, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.SwitchRadeonGpuUrl),
                callback);
        }
        #endregion

        #region GetLuckyUpdaterUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetLuckyUpdaterUrlAsync(Action<string, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.LuckyUpdaterUrl),
                callback,
                timeountMilliseconds: 5000);
        }
        #endregion

        #region GetMinerFinderUrlAsync
        public void GetMinerFinderUrlAsync(Action<string, Exception> callback)
        {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.MinerFinderUrl),
                callback,
                timeountMilliseconds: 5000);
        }
        #endregion

        #region GetPackageUrlAsync
        public void GetPackageUrlAsync(string package, Action<string, Exception> callback)
        {
            PackageUrlRequest request = new PackageUrlRequest
            {
                Package = package
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IPackageUrlController.PackageUrl),
                request,
                callback);
        }
        #endregion
    }
}
