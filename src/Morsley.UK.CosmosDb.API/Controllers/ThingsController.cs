namespace Morsley.UK.CosmosDb.API.Controllers;

[ApiController]
[Route("api/things")]
public class ThingsController : ControllerBase
{
    private readonly PersistenceService _service;

    public ThingsController(PersistenceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Thing>>> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(items);
    }
}