using System.ComponentModel.DataAnnotations;

namespace Routines.Api.Domain;

public class Action
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public string? Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public Guid? RoutineId { get; set; } = default!;
    
    public Routine? Routine { get; set; } = default!;
    
    public Guid? FollowUpActionId { get; set; } = default!;
    
    public Action? FollowUpAction { get; set; } = default!;
}
