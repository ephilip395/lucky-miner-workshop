using Lucky.Core.Kernels;
using Lucky.Core.Kernels.Impl;
using Lucky.Hub;
using System;
using System.Collections.Generic;

namespace Lucky.Core.Impl
{
    public class ServerContext : IServerContext
    {
        private readonly List<IMessagePathId> _contextPathIds = new List<IMessagePathId>();

        public ServerContext()
        {
            Init();
        }

        private void Init()
        {
            foreach (IMessagePathId pathId in _contextPathIds)
            {
                VirtualRoot.RemoveMessagePath(pathId);
            }
            _contextPathIds.Clear();
            CoinGroupSet = new CoinGroupSet(this);
            CoinSet = new CoinSet(this);
            FileWriterSet = new FileWriterSet(this);
            FragmentWriterSet = new FragmentWriterSet(this);
            GroupSet = new GroupSet(this);
            PoolSet = new PoolSet(this);
            SysDicItemSet = new SysDicItemSet(this);
            SysDicSet = new SysDicSet(this);
            CoinKernelSet = new CoinKernelSet(this);
            KernelInputSet = new KernelInputSet(this);
            KernelOutputSet = new KernelOutputSet(this);
            KernelOutputTranslaterSet = new KernelOutputTranslaterSet(this);
            KernelSet = new KernelSet(this);
            PackageSet = new PackageSet(this);
            PoolKernelSet = new PoolKernelSet(this);
        }

        public void ReInit()
        {
            Init();
            // CoreContext的视图模型集在此事件时刷新
            // 注意，ServerContext有15个Core层集合，所以必定有15个对应的Vm集合订阅该事件
            VirtualRoot.RaiseEvent(new ServerContextReInitedEvent());
        }

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public void AddCmdPath<TCmd>(LogEnum logType, Type location, Action<TCmd> path)
            where TCmd : ICmd
        {
            IMessagePathId messagePathId = VirtualRoot.BuildCmdPath(location, logType, path);
            _contextPathIds.Add(messagePathId);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public void AddEventPath<TEvent>(string description, LogEnum logType, Type location, PathPriority priority, Action<TEvent> path)
            where TEvent : IEvent
        {
            IMessagePathId messagePathId = VirtualRoot.BuildMessagePath(description, logType, location, priority, path);
            _contextPathIds.Add(messagePathId);
        }

        public ICoinGroupSet CoinGroupSet { get; private set; }

        public ICoinSet CoinSet { get; private set; }

        public IFileWriterSet FileWriterSet { get; private set; }

        public IFragmentWriterSet FragmentWriterSet { get; private set; }

        public IGroupSet GroupSet { get; private set; }

        public IPoolSet PoolSet { get; private set; }

        public ISysDicItemSet SysDicItemSet { get; private set; }

        public ISysDicSet SysDicSet { get; private set; }

        public ICoinKernelSet CoinKernelSet { get; private set; }

        public IKernelInputSet KernelInputSet { get; private set; }

        public IKernelOutputSet KernelOutputSet { get; private set; }

        public IKernelOutputTranslaterSet KernelOutputTranslaterSet { get; private set; }

        public IKernelSet KernelSet { get; private set; }

        public IPackageSet PackageSet { get; private set; }

        public IPoolKernelSet PoolKernelSet { get; private set; }
    }
}
