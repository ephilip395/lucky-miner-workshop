using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lucky.Rpc.Impl
{
    public partial class JsonRpcHelper : IJsonRpcHelper
    {
        public JsonRpcHelper() { }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">用于组装Url</param>
        /// <param name="port">用于组装Url</param>
        /// <param name="controller">用于组装Url</param>
        /// <param name="action">用于组装Url</param>
        /// <param name="query">Url上的查询参数，承载登录名、时间戳、签名</param>
        /// <param name="callback"></param>
        public void GetAsync<TResponse>(
            string host,
            int port,
            string controller,
            string action,
            Dictionary<string, string> query,
            Action<TResponse, Exception> callback,
            int? timeountMilliseconds = null)
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
                        Task<HttpResponseMessage> getHttpResponseTask = client.GetAsync(RpcRoot.GetUrl(host, port, controller, action, query));
                        if (getHttpResponseTask.Result.IsSuccessStatusCode)
                        {
                            getHttpResponseTask.Result.Content.ReadAsAsync<TResponse>().ContinueWith(t =>
                            {
                                callback?.Invoke(t.Result, null);
                            });
                        }
                        else
                        {
                            callback?.Invoke(default, new LuckyHttpException($"{action} http response {getHttpResponseTask.Result.StatusCode.ToString()} {getHttpResponseTask.Result.ReasonPhrase}")
                            {
                                StatusCode = getHttpResponseTask.Result.StatusCode,
                                ReasonPhrase = getHttpResponseTask.Result.ReasonPhrase
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    callback?.Invoke(default, e);
                }
            });
        }
    }
}
