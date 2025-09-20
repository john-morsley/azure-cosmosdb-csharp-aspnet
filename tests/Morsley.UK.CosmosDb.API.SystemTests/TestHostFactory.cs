namespace Morsley.UK.CosmosDb.API.SystemTests;

public sealed class TestHostFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _databaseId;

    public TestHostFactory()
    {
        _databaseId = $"morsley-uk-cosmos-db-tests-{Guid.NewGuid():N}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["Cosmos:Endpoint"] = "https://localhost:8081/",
                ["Cosmos:Key"] = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                ["Cosmos:DatabaseId"] = _databaseId,
                ["Cosmos:ContainerId"] = "things",
                ["Cosmos:PartitionKeyPath"] = "/id",
                ["Cosmos:IgnoreSslErrors"] = "true"
            };
            configBuilder.AddInMemoryCollection(overrides!);
        });

        builder.UseEnvironment("Development");
    }

    public async override ValueTask DisposeAsync()
    {
        // Cleanup database after tests complete
        var client = new CosmosClient(
            "https://localhost:8081/",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new CosmosClientOptions { ApplicationName = "SystemTestsCleanup" });
        try
        {
            var database = client.GetDatabase(_databaseId);
            await database.DeleteAsync();
        }
        catch
        {
            // ignore cleanup errors
        }
        client.Dispose();
        GC.SuppressFinalize(this);
    }
}