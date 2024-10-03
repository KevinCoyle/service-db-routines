namespace Routines.Api.Contracts.Responses.Actions;

public class ActionResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; init; } = default!;
    
    public string? Description { get; init; } = default!;
    
    public Guid RoutineId { get; init; } = default!;
    
    public Guid? FollowUpActionId { get; init; } = default!;
}
