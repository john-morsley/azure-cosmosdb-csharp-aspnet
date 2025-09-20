namespace Morsley.UK.CosmosDb.API.Documents;

public class Thing
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = UUIDNext.Uuid.NewDatabaseFriendly(UUIDNext.Database.Other).ToString();

    public string Name { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    public string Created { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffff");
}