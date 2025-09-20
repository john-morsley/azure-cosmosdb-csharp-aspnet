namespace Morsley.UK.CosmosDb.API.Models;

public class Thing
{
    public string? Id { get; set; }

    public string? Name { get; set; } = string.Empty;

    public string? Data { get; set; } = string.Empty;

    public string? Created { get; set; }
}