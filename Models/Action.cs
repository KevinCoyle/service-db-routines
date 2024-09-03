namespace RoutinesDbService.Models;

public sealed record Action()
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Guid RoutineId { get; set; }
    
    public Guid? FollowUpActionId { get; set; }
}