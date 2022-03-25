using Lucky.Rpc;
using Lucky.Rpc.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace Lucky
{
    public static partial class RpcRoot
    {
        public static readonly IJsonRpcHelper JsonRpc = new JsonRpcHelper();

        static RpcRoot()
        {
            SetOfficialServerAddress(LuckyKeyword.OfficialServerUrlBase);
        }

        public static string OfficialServerProtocol { get; private set; }
        public static string OfficialServerHost { get; private set; }
        public static int OfficialServerPort { get; private set; }
        public static string OfficialServerAddress { get; private set; }
        public static string OfficialServerUrlBase { get; private set; }
        public static void SetOfficialServerAddress(string address)
        {
            if (!address.Contains(":"))
            {
                OfficialServerProtocol = "http";
                OfficialServerHost = address;
                OfficialServerPort = LuckyKeyword.DefaultWebApiServerPort;
            }
            else if (address.StartsWith("http"))
            {

                var addressSplits = address.Split(':');
                if (addressSplits.Length == 2)
                {
                    OfficialServerProtocol = addressSplits[0];
                    OfficialServerHost = addressSplits[1].Substring(2);
                    if (OfficialServerProtocol == "https")
                    {
                        OfficialServerPort = 443;
                    }
                    else if (OfficialServerProtocol == "http")
                    {
                        OfficialServerPort = LuckyKeyword.DefaultWebApiServerPort;
                    }

                }
                else if (addressSplits.Length == 3)
                {
                    OfficialServerProtocol = addressSplits[0];
                    OfficialServerHost = addressSplits[1].Substring(2);
                    OfficialServerPort = int.Parse(addressSplits[2]);
                }
            }
            else
            {
                var addressSplits = address.Split(':');
                if (addressSplits.Length == 2)
                {
                    OfficialServerHost = addressSplits[0];
                    OfficialServerPort = int.Parse(addressSplits[1]);
                    if (OfficialServerPort == 443)
                    {
                        OfficialServerProtocol = "https";
                    }
                    else
                    {
                        OfficialServerProtocol = "http";
                    }
                }
            }
            if (OfficialServerProtocol == null || OfficialServerHost == null || OfficialServerPort.Equals(null))
            {
                throw new InvalidProgramException();
            }
            OfficialServerAddress = $"{OfficialServerHost}:{OfficialServerPort}";
            OfficialServerUrlBase = $"{OfficialServerProtocol}://{OfficialServerHost}:{OfficialServerPort}";
        }

        public static string GetUrl(string host, int port, string controller, string action, Dictionary<string, string> query)
        {
            string url = null;
            string queryString = string.Empty;
            if (query != null && query.Count != 0)
            {
                queryString = "?" + string.Join("&", query.Select(a => a.Key + "=" + a.Value));
            }
            if (port == 443)
            {
                url = $"https://{host}/api/{controller}/{action}{queryString}";
            }
            else if (port == 80)
            {
                url = $"http://{host}/api/{controller}/{action}{queryString}";
            }
            else
            {
                url = $"http://{host}:{port}/api/{controller}/{action}{queryString}";
            }
            LuckyConsole.DevDebug($"GetUrl : {url}");
            return url;
        }

        public static HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public static void SetTimeout(this HttpClient client, int? timeountMilliseconds)
        {
            if (!timeountMilliseconds.HasValue || timeountMilliseconds.Value < 0)
            {
                return;
            }
            if (timeountMilliseconds != 0)
            {
                if (timeountMilliseconds < 100)
                {
                    timeountMilliseconds *= 1000;
                }
                client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds.Value);
            }
        }

        public static byte[] ZipDecompress(byte[] zippedData)
        {
            using (Stream ms = new MemoryStream(zippedData),
                          compressedzipStream = new GZipStream(ms, CompressionMode.Decompress),
                          outBuffer = new MemoryStream())
            {
                byte[] block = new byte[LuckyKeyword.IntK];
                while (true)
                {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }
                    else
                    {
                        outBuffer.Write(block, 0, bytesRead);
                    }
                }
                compressedzipStream.Close();
                return ((MemoryStream)outBuffer).ToArray();
            }
        }
    }
}
