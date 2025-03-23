using System.ComponentModel;
using Routines.Api.Attributes;
using Routines.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Contracts.Requests.Routines;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
public class RoutineController : ControllerBase
{
    private readonly IRoutineService _routineService;

    public RoutineController(IRoutineService routineService)
    {
        _routineService = routineService;
    }

    [Description("Create a new Routine.")]
    [HttpPost("routines")]
    public async Task<IActionResult> Create([FromBody] RoutineRequest request)
    {
        var routine = request.ToRoutine();

        await _routineService.CreateAsync(routine);

        var routineResponse = routine.ToRoutineResponse();

        return CreatedAtAction("Get", new { routineResponse.Id }, routineResponse);
    }

    [Description("Get a Routine by ID.")]
    [HttpGet("routines/{id:guid}")]
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
    
    [Description("Get all Routines.")]
    [HttpGet("routines")]
    public async Task<IActionResult> GetAll()
    {
        var routines = await _routineService.GetAllAsync();
        var routinesResponse = routines.ToRoutinesResponse();
        return Ok(routinesResponse);
    }
    
    [Description("Update a Routine by ID.")]
    [HttpPut("routines/{id:guid}")]
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
    
    [Description("Delete a Routine by ID.")]
    [HttpDelete("routines/{id:guid}")]
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
