using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Routines.Api.Attributes;
using Routines.Api.Contracts.Requests.Schedules;
using Routines.Api.Mapping;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Controllers;

[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Description("Create a new Schedule.")]
    [HttpPost("schedules")]
    public async Task<IActionResult> Create([FromBody] ScheduleRequest request)
    {
        var schedule = request.ToSchedule();

        await _scheduleService.CreateAsync(schedule);

        var scheduleResponse = schedule.ToScheduleResponse();

        return CreatedAtAction("Get", new { scheduleResponse.Id }, scheduleResponse);
    }

    [Description("Get a Schedule by ID.")]
    [HttpGet("schedules/{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var schedule = await _scheduleService.GetAsync(id);

        if (schedule is null)
        {
            return NotFound();
        }

        var scheduleResponse = schedule.ToScheduleResponse();
        return Ok(scheduleResponse);
    }
    
    [Description("Get all Schedules.")]
    [HttpGet("schedules")]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _scheduleService.GetAllAsync();
        var schedulesResponse = schedules.ToSchedulesResponse();
        return Ok(schedulesResponse);
    }
    
    [Description("Update a Schedule by ID.")]
    [HttpPut("schedules/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateScheduleRequest request)
    {
        var schedule = await _scheduleService.GetAsync(request.Id);

        if (schedule is null)
        {
            return NotFound();
        }

        schedule.Name = request.Schedule.Name ?? schedule.Name;
        schedule.Description = request.Schedule.Description ?? schedule.Description;
        await _scheduleService.UpdateAsync(schedule);

        var scheduleResponse = schedule.ToScheduleResponse();
        return Ok(scheduleResponse);
    }
    
    [Description("Delete a Schedule by ID.")]
    [HttpDelete("schedules/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _scheduleService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
