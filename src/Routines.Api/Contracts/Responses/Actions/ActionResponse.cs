using Routines.Api.Contracts.Responses.Routines;

namespace Routines.Api.Contracts.Responses.Actions;

public class ActionResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; init; } = default!;
    
    public string? Description { get; init; } = default!;
    
    public Guid? RoutineId { get; init; }
    
    public RoutineResponse? Routine { get; init; }
    
    public Guid? FollowUpActionId { get; init; }
    
    public ActionResponse? FollowUpAction { get; init; }
}
