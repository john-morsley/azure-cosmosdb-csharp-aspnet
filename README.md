# azure-cosmosdb-csharp-aspnet
A minimal ASP.NET API that has CRUD endpoints to an Azure Cosmos database

## What was created
- **Solution**: `CosmosCrudApi.sln`
- **Project**: `src/CosmosCrudApi` targeting .NET 8
- **NuGet packages**:
  - `Microsoft.Azure.Cosmos` (Cosmos DB SQL API SDK)
  - `Newtonsoft.Json`
  - `Swashbuckle.AspNetCore` (Swagger)
- **Model**: class `ThingItem` in `Models/`
- **Service**: `Services/CosmosTodoService.cs` encapsulating Cosmos CRUD
- **Endpoints**: Minimal APIs in `Program.cs`
- **Config**: `appsettings.json` with `Cosmos` section (endpoint, key, database, container)

## Prerequisites
- .NET SDK 8
- An Azure Cosmos DB for NoSQL account OR the local Cosmos DB Emulator

## Configure credentials (recommended: User Secrets)
From the repository root, initialize user-secrets for the API project and set your Cosmos endpoint and key:

```powershell
# One-time init (from repo root)
dotnet user-secrets init --project .\src\CosmosCrudApi\CosmosCrudApi.csproj

# Set your Cosmos DB for NoSQL account values
# Example for a real account
 dotnet user-secrets set "Cosmos:Endpoint" "https://<your-account>.documents.azure.com:443/" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
 dotnet user-secrets set "Cosmos:Key" "<your-primary-key>" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj

# Optional: override defaults
 dotnet user-secrets set "Cosmos:DatabaseId" "CosmosCrudDb" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
 dotnet user-secrets set "Cosmos:ContainerId" "Things" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
 dotnet user-secrets set "Cosmos:PartitionKeyPath" "/id" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
```

Using Cosmos DB Emulator instead? Use:

```powershell
 dotnet user-secrets set "Cosmos:Endpoint" "https://localhost:8081/" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
 dotnet user-secrets set "Cosmos:Key" "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
```

You can also configure via environment variables at runtime:

```powershell
$env:Cosmos__Endpoint = "https://<your-account>.documents.azure.com:443/"
$env:Cosmos__Key = "<your-primary-key>"
$env:Cosmos__DatabaseId = "CosmosCrudDb"
$env:Cosmos__ContainerId = "Things"
$env:Cosmos__PartitionKeyPath = "/id"
```

## Run the API

```powershell
# Build
 dotnet build

# Run (from repo root)
 dotnet run --project .\src\CosmosCrudApi\CosmosCrudApi.csproj
```

By default Swagger UI is enabled in Development at:
- https://localhost:5001/swagger
- or http://localhost:5000/swagger

## REST Endpoints

- `GET    /api/things` — list all
- `GET    /api/things/{id}` — get by id
- `POST   /api/things` — create
- `PUT    /api/things/{id}` — upsert/replace
- `DELETE /api/things/{id}` — delete

### Example requests (PowerShell)

```powershell
# Create
$body = @{ title = "First thing"; isComplete = $false } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri https://localhost:5001/api/things -ContentType 'application/json' -Body $body

# List
Invoke-RestMethod -Method Get -Uri https://localhost:5001/api/things

# Get by id
$id = "<id-from-create>"
Invoke-RestMethod -Method Get -Uri https://localhost:5001/api/things/$id

# Update
$update = @{ id=$id; title = "First thing (updated)"; isComplete = $true } | ConvertTo-Json
Invoke-RestMethod -Method Put -Uri https://localhost:5001/api/things/$id -ContentType 'application/json' -Body $update

# Delete
Invoke-RestMethod -Method Delete -Uri https://localhost:5001/api/things/$id
```

## Notes
- The container is auto-provisioned if it does not exist using partition key path `/id`.
- `ThingItem.Id` is used as the partition key; this is fine for small workloads or demos. For production, choose a partition key that evenly distributes throughput and storage.
- The SDK used is `Microsoft.Azure.Cosmos` v3.x which is fully compatible with .NET 8.

