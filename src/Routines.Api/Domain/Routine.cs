using System.ComponentModel.DataAnnotations;

namespace Routines.Api.Domain;

public class Routine
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public string? Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public Routine? FollowUpRoutine { get; set; } = default!;
    
    public Guid? OwnerId { get; set; } = default!;
    
    public User? Owner { get; set; } = default!;
    
    public IEnumerable<Action>? Actions { get; set; } = default!;
    
    public IEnumerable<Schedule>? Schedules { get; set; } = default!;
}
