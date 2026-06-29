#region

using Newtonsoft.Json;

#endregion

namespace Amp.Data.Entities;

public abstract class BaseDocument
{
    protected BaseDocument(string type, string partitionKey)
    {
        Type = type;
        PartitionKey = partitionKey;
    }

    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastModifiedAt")]
    public DateTime? LastModifiedAt { get; set; }

    [JsonProperty("_etag")]
    public string ETag { get; set; } = string.Empty;
}
