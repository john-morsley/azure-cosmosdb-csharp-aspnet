namespace Morsley.UK.CosmosDb.API.Controllers;

[ApiController]
[Route("api/thing")]
public class ThingController : ControllerBase
{
    private readonly PersistenceService _service;

    public ThingController(PersistenceService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Thing>> GetById(string id, CancellationToken ct)
    {
        var thing = await _service.GetByIdAsync(id, ct);
        return thing is null ? NotFound() : Ok(thing);
    }

    [HttpPost]
    public async Task<ActionResult<Thing>> Create([FromBody] Thing thing, CancellationToken ct)
    {
        var created = await _service.CreateAsync(thing, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Thing>> Upsert(string id, [FromBody] Thing thing, CancellationToken ct)
    {
        var updated = await _service.UpsertAsync(id, thing, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}