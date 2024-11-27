namespace Routines.Api.Contracts.Requests.Schedules;

public class ScheduleRequest
{
    public Guid? Id { get; set; } = default!;
    public string? Name { get; init; } = default!;
    public string? Description { get; init; } = default!;
}