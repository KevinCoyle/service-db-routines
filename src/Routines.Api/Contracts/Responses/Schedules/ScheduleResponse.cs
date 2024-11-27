using Routines.Api.Domain;

namespace Routines.Api.Contracts.Responses.Schedules;

public class ScheduleResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; init; } = default!;
    
    public string? Description { get; init; } = default!;
    
    public IEnumerable<RelativeInterval>? Intervals { get; init; }
}
