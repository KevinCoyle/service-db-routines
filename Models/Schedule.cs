namespace RoutinesDbService.Models;

public class Schedule()
{
    public Guid? Id { get; set; }
    public Guid? RoutineId { get; set; }
    public Routine? Routine { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<RelativeInterval>? Intervals { get; set; }
}