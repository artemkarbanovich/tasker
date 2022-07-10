
namespace Tasker.Core.Interfaces.Services;

public interface IHttpService
{
    Task<string> HttpGet(string url, Dictionary<string, string> headers);
}
