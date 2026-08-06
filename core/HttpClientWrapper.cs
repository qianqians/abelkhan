namespace core;

public class HttpClientWrapper
{
    private static readonly HttpClient Client = new HttpClient();
    public static void Init()
    {
        Client.Timeout = TimeSpan.FromSeconds(3);
    }

    public static async ValueTask<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        try
        {
            HttpResponseMessage response = await Client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (HttpRequestException e)
        {
            Log.Error("HttpClientWrapper.PostAsync url: {0} error: {1}", url, e);
            throw;
        }
    }

    public static async ValueTask<HttpResponseMessage> GetRspAsync(string url, List<KeyValuePair<string, string>>? headers = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (headers is { Count: > 0 })
            {
                foreach (var (key, value) in headers)
                {
                    request.Headers.Add(key, value);
                }
            }
            HttpResponseMessage response = await Client.SendAsync(request);
            return response;
        }
        catch (HttpRequestException e)
        {
            Log.Error("HttpClientWrapper.GetRspAsync url: {0} error: {1}", url, e);
            throw;
        }
    }
}