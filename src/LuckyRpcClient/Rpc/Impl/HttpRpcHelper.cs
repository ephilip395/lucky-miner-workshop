﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lucky.Rpc.Impl
{
    public class HttpRpcHelper : IHttpRpcHelper
    {
        public HttpRpcHelper() { }

        /// <summary>
        /// 注意：Request时原始HttpContent，Fire忽略Response
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="query"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        /// <param name="timeountMilliseconds"></param>
        public void FirePostAsync(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            HttpContent content,
            Action callback = null,
            int timeountMilliseconds = 0)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (HttpClient client = RpcRoot.CreateHttpClient())
                    {
                        if (timeountMilliseconds > 0)
                        {
                            client.SetTimeout(timeountMilliseconds);
                        }
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsync(RpcRoot.GetUrl(host, port, controller, action, query), content);
                        if (!getHttpResponse.Result.IsSuccessStatusCode)
                        {
                            LuckyConsole.DevDebug($"{action} http response {getHttpResponse.Result.StatusCode.ToString()} {getHttpResponse.Result.ReasonPhrase}");
                        }
                        callback?.Invoke();
                    }
                }
                catch
                {
                    callback?.Invoke();
                }
            });
        }
    }
}
