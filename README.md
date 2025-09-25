# Azure Cosmos DB ASP.NET API in C#

A minimal ASP.NET API that has CRUD endpoints to an Azure Cosmos database

## Local Development

The User Secrets file needs the following:

```json
{
  "Cosmos:Endpoint": "",
  "Cosmos:Key": ""
}
```

To obtain these values see the following 2 sections...

### Local Database

During local development the Azure Cosmos DB Emulator from Microosft was used.
Once this application is running, it'll open open an administration web page on:

https://localhost:8081/_explorer/index.html

From here you can take the URI and Primary Key, and put them into the API projects User Secrets file.

### Remote Database

URI: Azure Cosmos DB Account -> Settings -> Keys -> URI  
Primary Key: Azure Cosmos DB Account -> Settings -> Keys  

Here you may select either primary or secondry key, but for this project it must be a read-write key.