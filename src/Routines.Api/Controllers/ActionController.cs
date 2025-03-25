using System.ComponentModel;
using Routines.Api.Attributes;
using Routines.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Contracts.Requests.Actions;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]s")]
public class ActionController : ControllerBase
{
    private readonly IActionService _actionService;

    public ActionController(IActionService actionService)
    {
        _actionService = actionService;
    }

    [HttpPost]
    [Description("Create a new Action.")]
    public async Task<IActionResult> Create([FromBody] ActionRequest request)
    {
        var action = request.ToAction();

        await _actionService.CreateAsync(action);

        var actionResponse = action.ToActionResponse();

        return CreatedAtAction("Get", new { actionResponse.Id }, actionResponse);
    }

    [HttpGet("{id:guid}")]
    [Description("Get an Action by ID.")]
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
    
    [HttpGet]
    [Description("Get All Actions.")]
    public async Task<IActionResult> GetAll()
    {
        var actions = await _actionService.GetAllAsync();
        var actionsResponse = actions.ToActionsResponse();
        return Ok(actionsResponse);
    }
    
    [HttpPut]
    [Description("Update an Action by ID.")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateActionRequest request)
    {
        var action = await _actionService.GetAsync(request.Id);

        if (action is null)
        {
            return NotFound();
        }

        action.Name = request.Action.Name;
        await _actionService.UpdateAsync(action);

        var actionResponse = action.ToActionResponse();
        return Ok(actionResponse);
    }
    
    [HttpDelete("{id:guid}")]
    [Description("Delete an Action by ID.")]
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
