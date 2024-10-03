namespace Routines.Api.Contracts.Requests.Schedules;

public class ScheduleRequest
{
    public Guid? Id { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
}