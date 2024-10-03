using Routines.Api.Contracts.Responses.Routines;
using Routines.Api.Domain;

namespace Routines.Api.Contracts.Responses.Schedules;

public class ScheduleResponse
{
    public Guid Id { get; init; }
    
    public Guid? RoutineId { get; set; } = default!;
    
    public RoutineResponse? Routine { get; set; } = default!;
    
    public string? Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public IEnumerable<RelativeInterval>? Intervals { get; set; } = default!;
}
