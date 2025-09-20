namespace Morsley.UK.CosmosDb.API.Extensions;

public static class ThingExtensions
{
    public static Models.Thing ToModel(this Documents.Thing document)
    {
        var model = new Models.Thing();
        model.Id = document.Id.ToString();
        model.Name = document.Name;
        model.Data = document.Data;
        if (!string.IsNullOrEmpty(document.Created))
        {            
            if (DateTime.TryParse(document.Created, out var created))
            {
                model.Created = created.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            }
        }
        return model;
    }

    public static IEnumerable<Models.Thing> ToModels(this IEnumerable<Documents.Thing> documents)
    {
        var models = new List<Models.Thing>();
        foreach (var document in documents)
        {
            models.Add(document.ToModel());
        }
        return models;
    }

    public static IList<Models.Thing> ToModels(this IList<Documents.Thing> documents)
    {
        var models = new List<Models.Thing>();
        foreach (var document in documents)
        {
            models.Add(document.ToModel());
        }
        return models;
    }

    public static Documents.Thing ToDocument(this Models.Thing model)
    {
        var document = new Documents.Thing
        {
            Name = model.Name ?? string.Empty,
            Data = model.Data ?? string.Empty,
        };

        if (!string.IsNullOrEmpty(model.Id))
        {
            document.Id = model.Id;
        }

        if (!string.IsNullOrEmpty(model.Created))
        {            
            if (DateTime.TryParse(model.Created, out var created))
            {
                document.Created = created.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            }
        }

        return document;
    }

    public static IEnumerable<Documents.Thing> ToDocuments(this IEnumerable<Models.Thing> models)
    {
        var documents = new List<Documents.Thing>();
        foreach (var model in models)
        {
            documents.Add(model.ToDocument());
        }
        return documents;
    }
}