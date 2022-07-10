using Tasker.Core.Interfaces.Services;

namespace Tasker.Infrastructure.Services;

public class HttpService : IHttpService
{
    public async Task<string> HttpGet(string url, Dictionary<string, string> headers)
    {
        var httpClient = new HttpClient();

        foreach (var header in headers)
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

        var response = await httpClient.GetAsync(url);

        return await response.Content.ReadAsStringAsync();
    }
}
