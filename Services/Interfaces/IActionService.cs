using Action = RoutinesDbService.Models.Action;

namespace RoutinesDbService.Services.Interfaces;

public interface IActionService
{
    Task<List<Action>> GetAllActions();
    Task<Action?> GetActionById(string id);
    Task<bool> CreateAction(Action action);
    Task<bool> UpdateAction(Action action);
    Task<bool> DeleteAction(string id);
}