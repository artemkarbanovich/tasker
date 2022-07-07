using Tasker.Core.Exceptions;

namespace Tasker.Core.Entities;

public class FreeApi
{
    public FreeApi(
        string id,
        string apiUrl,
        string name,
        string apiDescription,
        string apiIconUrl,
        string rapidApiHost,
        bool isQueryRequired,
        string queryKey,
        string queryDescription)
    {
        Id = id;
        ApiUrl = apiUrl;
        Name = name;
        ApiDescription = apiDescription;
        ApiIconUrl = apiIconUrl;
        RapidApiHost = rapidApiHost;
        IsQueryRequired = isQueryRequired;

        if(IsQueryRequired && queryKey is null)
            throw new DefaultException("Query key is required");

        if (IsQueryRequired && queryDescription is null)
            throw new DefaultException("Query descriprion is required");

        QueryKey = queryKey;
        QueryDescription = queryDescription;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string ApiUrl { get; private set; }
    public string Name { get; private set; }
    public string ApiDescription { get; private set; }
    public string? ApiIconUrl { get; private set; }
    public string RapidApiHost { get; private set; }
    public bool IsQueryRequired { get; private set; }
    public string? QueryKey { get; private set; }
    public string? QueryDescription { get; private set; }
}
