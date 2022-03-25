using Lucky.Serialization;
using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Lucky
{
    public static class HttpServer
    {
        private static HttpSelfHostServer s_httpServer;
        public static void Start(string baseAddress, Action<HttpSelfHostConfiguration> doConfig = null)
        {
            if (s_httpServer != null)
            {
                return;
            }
            try
            {
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(baseAddress);
                config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
                config.Formatters.JsonFormatter.SerializerSettings = NTJsonSerializer.SerializerSettings;
                doConfig?.Invoke(config);
                // 在默认路由之前 doConfig 从而使自定义路由的优先级高于默认路由
                config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}");
                s_httpServer = new HttpSelfHostServer(config);
                s_httpServer.OpenAsync();
                VirtualRoot.BuildEventPath<AppExitEvent>("退出HttpServer", LogEnum.None, typeof(HttpServer), PathPriority.Normal, path: message =>
                  {
                      HttpSelfHostServer tmp = s_httpServer;
                      if (tmp != null)
                      {
                          s_httpServer = null;
                          tmp.Dispose();
                      }
                  });
            }
            catch (Exception e)
            {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
