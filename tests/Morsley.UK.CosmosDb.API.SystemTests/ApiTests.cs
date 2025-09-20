namespace Morsley.UK.CosmosDb.API.SystemTests;

public class ApiTests : IClassFixture<TestHostFactory>
{
    private readonly HttpClient _client;

    public ApiTests(TestHostFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    /*                                                    \\\!///
                                                        \\  - -  //
                                                         (  @ @  )
    +--------------------------------------------------oOOo-(_)-oOOo--------------------------------------------------+
    !                                                                                                                 !
    ! This, I know, rather long test, tests the CRUD capabilities of the API. It does it, in what I hope, is an       !
    ! understandable logical order. It utilises the Azure Cosmos DB Emulator, from Microsoft. A database instance is  !
    ! created, a container is created, the tests are run, and finally the database, and therefore its container too,  ! 
    ! is deleted.                                                                                                     !
    !                                                                                                                 !
    +-----------------------------------------------------------Oooo--------------------------------------------------+
                                                       oooO    (   )
                                                       (   )    ) /
                                                        \ (    (_/
                                                         \_)                                                         */
    [Fact]
    public async Task when_calling_all_api_endpoints_in_a_logical_order_then_it_should_exhibit_expected_crud_capabilities()
    {
        // Arrange...
        var name1 = "Test A";
        var data1 = "Alpha";
        var name2 = "Test B";
        var data2 = "Bravo";

        var thingApi = "/api/thing";
        var thingsApi = "/api/things";

        // GET - All things - Should return nothing - 200: OK
        var getAllResponse = await _client.GetAsync(thingsApi);
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var things = await getAllResponse.Content.ReadFromJsonAsync<List<Models.Thing>>();
        things.ShouldNotBeNull();
        things.Count.ShouldBe(0);

        // POST - Create a new thing - 201: Created
        var modelToCreate = new Models.Thing
        {
            Name = name1,
            Data = data1,
        };
        var createResponse = await _client.PostAsJsonAsync(thingApi, modelToCreate);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<Models.Thing>();
        created.ShouldNotBeNull();
        created!.Id.ShouldNotBeNullOrWhiteSpace();
        created.Name.ShouldBe(name1);
        created.Data.ShouldBe(data1);

        // GET - By Id - Get the thing we just created
        var getByIdResponse = await _client.GetAsync($"{thingApi}/{created.Id}");
        getByIdResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var fetched = await getByIdResponse.Content.ReadFromJsonAsync<Models.Thing>();
        fetched.ShouldNotBeNull();
        fetched!.Id.ShouldBe(created.Id);
        fetched.Name.ShouldBe(name1);
        fetched.Data.ShouldBe(data1);
        fetched.Created.ShouldBe(created.Created);

        // GET - All things - Should return our single thing - 200: OK
        getAllResponse = await _client.GetAsync(thingsApi);
        getAllResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        things = await getAllResponse.Content.ReadFromJsonAsync<List<Models.Thing>>();
        things.ShouldNotBeNull();
        things.Count.ShouldBe(1);
        var thing = things[0];
        thing.Id.ShouldBe(created.Id);
        thing.Name.ShouldBe(name1);
        thing.Data.ShouldBe(data1);
        thing.Created.ShouldBe(created.Created);

        // GET - By Id - Get the thing we just created - 200: OK
        getByIdResponse = await _client.GetAsync($"{thingApi}/{created.Id}");
        getByIdResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        fetched = await getByIdResponse.Content.ReadFromJsonAsync<Models.Thing>();
        fetched.ShouldNotBeNull();
        fetched!.Id.ShouldBe(created.Id);
        fetched.Name.ShouldBe(name1);
        fetched.Data.ShouldBe(data1);
        fetched.Created.ShouldBe(created.Created);

        // PUT (Upsert) - Update our thing - 200: OK
        var modelToUpdate = new Models.Thing
        {
            Id = created.Id,
            Name = name2,
            Data = data2
        };
        var upsertResponse = await _client.PutAsJsonAsync($"{thingApi}/{created.Id}", modelToUpdate);
        upsertResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updated = await upsertResponse.Content.ReadFromJsonAsync<Models.Thing>();
        updated.ShouldNotBeNull();
        fetched!.Id.ShouldBe(created.Id);
        updated.Name.ShouldBe(name2);
        updated.Data.ShouldBe(data2);
        updated.Created.ShouldBe(created.Created);

        // DELETE - Speaks for itself - 204: No Content
        var deleteResponse = await _client.DeleteAsync($"{thingApi}/{created.Id}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // GET - By Id - Get the thing we just deleted - 404: Not Found
        var getMissing = await _client.GetAsync($"{thingApi}/{created.Id}");
        getMissing.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}