﻿using Lucky.Rpc;
using Lucky.Rpc.Impl;
using Lucky.Services;
using System;

namespace Lucky
{
    public static partial class RpcRoot
    {
        public static RpcUser RpcUser { get; private set; } = RpcUser.Empty;
        public static DateTime LoginedOn { get; private set; }
        public static bool IsOuterNet { get; private set; } = false;
        public static bool IsInnerNet
        {
            get
            {
                return !IsOuterNet;
            }
        }
        public static bool IsLogined
        {
            get
            {
                return RpcUser == null || RpcUser == RpcUser.Empty
                    ? false
                    : !string.IsNullOrEmpty(RpcUser.LoginName) && !string.IsNullOrEmpty(RpcUser.Password);
            }
        }

        public static void Login(RpcUser rpcUser)
        {
            if (RpcUser != null)
            {
                RpcUser.Logout();
            }
            RpcUser = rpcUser;
            LoginedOn = DateTime.Now;
        }

        public static void SetIsOuterNet(bool value)
        {
            bool isChanged = IsOuterNet != value;
            IsOuterNet = value;
            if (isChanged)
            {
                VirtualRoot.RaiseEvent(new MinerMonitorServiceSwitchedEvent(value ? MinerMonitorServiceType.Out : MinerMonitorServiceType.Local));
            }
        }

        private static bool _isServerMessagesVisible = false;
        /// <summary>
        /// 表示服务器消息在界面上当前是否是可见的。true表示是可见的，反之不是。
        /// </summary>
        /// <remarks>本地会根据服务器消息在界面山是否可见优化网络传输，不可见的时候不从服务器加载消息。</remarks>
        public static bool IsServerMessagesVisible
        {
            get { return _isServerMessagesVisible; }
        }

        // 独立一个方法是为了方便编程工具走查代码，这算是个模式吧，不只出现这一次。编程的用户有三个：1，人；2，编程工具；3，运行时；
        public static void SetIsServerMessagesVisible(bool value)
        {
            _isServerMessagesVisible = value;
        }

        public static readonly IBinaryRequestJsonResponseRpcHelper BinaryRequestJsonResponseRpcHelper = new BinaryRequestJsonResponseRpcHelper();
        public static readonly IHttpRpcHelper HttpRpcHelper = new HttpRpcHelper();
        public static readonly IJsonRequestBinaryResponseRpcHelper JsonRequestBinaryResponseRpcHelper = new JsonRequestBinaryResponseRpcHelper();

        public static readonly OfficialServices OfficialServer = new OfficialServices();
        public static readonly ClientServices Client = new ClientServices();
        public static readonly OSSService OSSService = new OSSService();
    }
}
