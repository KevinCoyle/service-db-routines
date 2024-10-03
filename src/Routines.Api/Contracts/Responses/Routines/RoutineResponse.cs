using Routines.Api.Contracts.Responses.Actions;
using Routines.Api.Contracts.Responses.Schedules;
using Routines.Api.Contracts.Responses.Users;

namespace Routines.Api.Contracts.Responses.Routines;

public class RoutineResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public RoutineResponse? FollowUpRoutine { get; set; } = default!;
    
    public Guid? OwnerId { get; set; } = default!;
    
    public UserResponse? Owner { get; set; } = default!;
    
    public IEnumerable<ActionResponse>? Actions { get; set; } = default!;
    
    public IEnumerable<ScheduleResponse>? Schedules { get; set; } = default!;
}
