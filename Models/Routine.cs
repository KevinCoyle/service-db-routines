namespace RoutinesDbService.Models;

public class Routine()
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Routine? FollowUpRoutine { get; set; }
    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; }
    public List<Action>? Actions { get; set; }
    public IReadOnlyList<Schedule>? Schedules { get; set; }
}