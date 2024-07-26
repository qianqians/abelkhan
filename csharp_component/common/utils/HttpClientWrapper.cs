using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Abelkhan
{
    public sealed class HttpClientWrapper
    {
        static readonly HttpClient client = new HttpClient();
        public static void Init()
        {
            client.Timeout = TimeSpan.FromSeconds(3);
        }

        public static async ValueTask<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException e)
            {
                Log.Log.err("HttpClientWrapper.PostAsync url: {0} error: {1}", url, e);
                throw;
            }
        }

        public static async ValueTask<HttpResponseMessage> GetRspAsync(string url, List<KeyValuePair<string, string>> headers = null)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }
                    HttpResponseMessage response = await client.SendAsync(request);
                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Log.Log.err("HttpClientWrapper.GetRspAsync url: {0} error: {1}", url, e);
                throw;
            }
        }
    }
}