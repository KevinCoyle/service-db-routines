using System.ComponentModel;
using Routines.Api.Attributes;
using Routines.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Contracts.Requests.Routines;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]s")]
public class RoutineController : ControllerBase
{
    private readonly IRoutineService _routineService;

    public RoutineController(IRoutineService routineService)
    {
        _routineService = routineService;
    }

    [HttpPost]
    [Description("Create a new Routine.")]
    public async Task<IActionResult> Create([FromBody] RoutineRequest request)
    {
        var routine = request.ToRoutine();

        await _routineService.CreateAsync(routine);

        var routineResponse = routine.ToRoutineResponse();

        return CreatedAtAction("Get", new { routineResponse.Id }, routineResponse);
    }

    [HttpGet("{id:guid}")]
    [Description("Get a Routine by ID.")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var routine = await _routineService.GetAsync(id);

        if (routine is null)
        {
            return NotFound();
        }

        var routineResponse = routine.ToRoutineResponse();
        return Ok(routineResponse);
    }
    
    [HttpGet]
    [Description("Get all Routines.")]
    public async Task<IActionResult> GetAll()
    {
        var routines = await _routineService.GetAllAsync();
        var routinesResponse = routines.ToRoutinesResponse();
        return Ok(routinesResponse);
    }
    
    [HttpPut("{id:guid}")]
    [Description("Update a Routine by ID.")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateRoutineRequest request)
    {
        var routine = await _routineService.GetAsync(request.Id);

        if (routine is null)
        {
            return NotFound();
        }

        routine.Name = request.Routine.Name ?? routine.Name;
        routine.OwnerId = request.Routine.OwnerId ?? routine.OwnerId;
        await _routineService.UpdateAsync(routine);

        var routineResponse = routine.ToRoutineResponse();
        return Ok(routineResponse);
    }
    
    [HttpDelete("{id:guid}")]
    [Description("Delete a Routine by ID.")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _routineService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
