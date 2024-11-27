namespace Routines.Api.Contracts.Requests.Routines;

public class RoutineRequest
{
    public Guid? Id { get; set; } = default!;
    
    public string? Name { get; init; } = default!;
    
    public Guid? OwnerId { get; init; } = default!;
}