using Routines.Api.Attributes;
using Routines.Api.Mapping;
using Routines.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Contracts.Requests.Actions;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
public class ActionController : ControllerBase
{
    private readonly IActionService _actionService;

    public ActionController(IActionService actionService)
    {
        _actionService = actionService;
    }

    [HttpPost("actions")]
    public async Task<IActionResult> Create([FromBody] ActionRequest request)
    {
        var action = request.ToAction();

        await _actionService.CreateAsync(action);

        var actionResponse = action.ToActionResponse();

        return CreatedAtAction("Get", new { actionResponse.Id }, actionResponse);
    }

    [HttpGet("actions/{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var action = await _actionService.GetAsync(id);

        if (action is null)
        {
            return NotFound();
        }

        var actionResponse = action.ToActionResponse();
        return Ok(actionResponse);
    }
    
    [HttpGet("actions")]
    public async Task<IActionResult> GetAll()
    {
        var actions = await _actionService.GetAllAsync();
        var actionsResponse = actions.ToActionsResponse();
        return Ok(actionsResponse);
    }
    
    [HttpPut("actions/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateActionRequest request)
    {
        var existingAction = await _actionService.GetAsync(request.Id);

        if (existingAction is null)
        {
            return NotFound();
        }

        var action = request.ToAction();
        await _actionService.UpdateAsync(action);

        var actionResponse = action.ToActionResponse();
        return Ok(actionResponse);
    }
    
    [HttpDelete("actions/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _actionService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
