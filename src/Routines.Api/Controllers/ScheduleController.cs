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

    [HttpPost("schedules")]
    public async Task<IActionResult> Create([FromBody] ScheduleRequest request)
    {
        var schedule = request.ToSchedule();

        await _scheduleService.CreateAsync(schedule);

        var scheduleResponse = schedule.ToScheduleResponse();

        return CreatedAtAction("Get", new { scheduleResponse.Id }, scheduleResponse);
    }

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
    
    [HttpGet("schedules")]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _scheduleService.GetAllAsync();
        var schedulesResponse = schedules.ToSchedulesResponse();
        return Ok(schedulesResponse);
    }
    
    [HttpPut("schedules/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateScheduleRequest request)
    {
        var existingSchedule = await _scheduleService.GetAsync(request.Id);

        if (existingSchedule is null)
        {
            return NotFound();
        }

        var schedule = request.ToSchedule();
        await _scheduleService.UpdateAsync(schedule);

        var scheduleResponse = schedule.ToScheduleResponse();
        return Ok(scheduleResponse);
    }
    
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
