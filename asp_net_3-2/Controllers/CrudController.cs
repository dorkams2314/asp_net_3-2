using System.ComponentModel.DataAnnotations;
using System.Globalization;
using asp_net_3_2.DTOs;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[ApiController]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
public abstract class CrudController<TDto, TCreate, TUpdate, TPatch>(IService<TDto, TCreate, TUpdate, TPatch> service)
    : ControllerBase where TDto : IResourceDto
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TDto>>> GetAll(
        [Range(1, int.MaxValue / 100)] int page = 1, [Range(1, 100)] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetPageAsync(page, pageSize, cancellationToken);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString(CultureInfo.InvariantCulture);
        return Ok(result.Items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TDto>> Create([FromBody] TCreate dto, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] TUpdate dto, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Patch(int id, [FromBody] TPatch dto, CancellationToken cancellationToken)
    {
        await service.PatchAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
