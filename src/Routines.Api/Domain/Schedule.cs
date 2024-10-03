using System.ComponentModel.DataAnnotations;

namespace Routines.Api.Domain;

public class Schedule
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid? RoutineId { get; set; } = default!;
    
    public Routine? Routine { get; set; } = default!;
    
    public string? Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public IEnumerable<RelativeInterval>? Intervals { get; set; } = default!;
}
