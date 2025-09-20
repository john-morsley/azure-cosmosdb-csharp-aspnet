namespace Morsley.UK.CosmosDb.API.Services;

public class PersistenceService
{
    private readonly CosmosClient _client;
    private readonly string _databaseId;
    private readonly string _containerId;
    private readonly string _partitionKeyPath;

    private Database? _database;
    private Container? _container;

    public PersistenceService(IConfiguration configuration)
    {
        var endpoint = configuration["Cosmos:Endpoint"] ?? string.Empty;
        var key = configuration["Cosmos:Key"] ?? string.Empty;
        _databaseId = configuration["Cosmos:DatabaseId"] ?? "CosmosCrudDb";
        _containerId = configuration["Cosmos:ContainerId"] ?? "Things";
        _partitionKeyPath = configuration["Cosmos:PartitionKeyPath"] ?? "/id";

        var options = new CosmosClientOptions
        {
            ApplicationName = "Morsley.UK.CosmosDb.API",
        };

        var ignoreSsl = string.Equals(configuration["Cosmos:IgnoreSslErrors"], "true", StringComparison.OrdinalIgnoreCase);
        if (ignoreSsl || endpoint.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase))
        {
            options.HttpClientFactory = () =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(handler, disposeHandler: true);
            };
        }

        _client = new CosmosClient(endpoint, key, options);
    }

    private async Task<Container> GetContainerAsync(CancellationToken ct)
    {
        if (_container is not null)
            return _container;

        _database = await _client.CreateDatabaseIfNotExistsAsync(_databaseId, cancellationToken: ct);
        var containerResponse = await _database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = _containerId,
            PartitionKeyPath = _partitionKeyPath
        }, cancellationToken: ct);

        _container = containerResponse.Container;
        return _container;
    }

    public async Task<IEnumerable<Thing>> GetAllAsync(CancellationToken ct = default)
    {
        var container = await GetContainerAsync(ct);
        var query = new QueryDefinition("SELECT * FROM c");
        using var iterator = container.GetItemQueryIterator<Documents.Thing>(query);        
        var documents = new List<Documents.Thing>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(ct);
            documents.AddRange(response.ToList());
        }
        var models = documents.ToModels();
        return models;
    }

    public async Task<Thing?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var container = await GetContainerAsync(ct);
        try
        {
            var response = await container.ReadItemAsync<Documents.Thing>(id, new PartitionKey(id), cancellationToken: ct);
            var documentFound = response.Resource;
            var modelFound = documentFound.ToModel();
            return modelFound;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Thing> CreateAsync(Thing modelToCreate, CancellationToken ct = default)
    {
        modelToCreate.Id = null;
        modelToCreate.Created = null;
        var documentToCreate = modelToCreate.ToDocument();
        var container = await GetContainerAsync(ct);
        var response = await container.CreateItemAsync<Documents.Thing>(documentToCreate, new PartitionKey(documentToCreate.Id), cancellationToken: ct);
        var documnetCreated = response.Resource;
        var modelCreated = documnetCreated.ToModel();
        return modelCreated;
    }

    public async Task<Thing?> UpsertAsync(string id, Thing modelToUpsert, CancellationToken ct = default)
    {
        modelToUpsert.Id = id;
        modelToUpsert.Created = null;
        var exists = await GetByIdAsync(id, ct);
        if (exists != null)
        {
            modelToUpsert.Created = exists.Created;
        }
        var documentToUpsert = modelToUpsert.ToDocument();
        var container = await GetContainerAsync(ct);
        var response = await container.UpsertItemAsync<Documents.Thing>(documentToUpsert, new PartitionKey(id), cancellationToken: ct);
        var documentUpserted = response.Resource;
        var modelUpserted = documentUpserted.ToModel();
        return modelUpserted;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var container = await GetContainerAsync(ct);
        try
        {
            await container.DeleteItemAsync<Documents.Thing>(id, new PartitionKey(id), cancellationToken: ct);
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}