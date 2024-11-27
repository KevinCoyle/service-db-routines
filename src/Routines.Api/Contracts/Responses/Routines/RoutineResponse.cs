using Routines.Api.Contracts.Responses.Actions;
using Routines.Api.Contracts.Responses.Schedules;
using Routines.Api.Contracts.Responses.Users;

namespace Routines.Api.Contracts.Responses.Routines;

public class RoutineResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; init; } = default!;
    
    public string? Description { get; init; } = default!;
    
    public Guid? OwnerId { get; init; }
    
    public UserResponse? Owner { get; init; }
    
    public IEnumerable<ActionResponse>? Actions { get; init; }
    
    public IEnumerable<ScheduleResponse>? Schedules { get; init; } 
    
    public RoutineResponse? FollowUpRoutine { get; init; }
}
