using RoutinesDbService.Repositories;
using RoutinesDbService.Services.Interfaces;
using Action = RoutinesDbService.Models.Action;

namespace RoutinesDbService.Services.Implementations;

public class ActionService : IActionService
{
    public async Task<List<Action>> GetAllActions()
    {
        var results = await _actionsRepository.GetAll();
        
        return results;
    }

    public async Task<Action?> GetActionById(string id)
    {
        var result = await _actionsRepository.GetById(id);
        
        return result;
    }

    public async Task<bool> CreateAction(Action action)
    {
        var result = await _actionsRepository.Create(action);
        
        return result;
    }

    public async Task<bool> UpdateAction(Action action)
    {
        var result = await _actionsRepository.Update(action);
        
        return result;
    }

    public async Task<bool> DeleteAction(string id)
    {
        var result = await _actionsRepository.Delete(id);
        
        return result;
    }
    
#region Constructor

    private readonly ActionsRepository _actionsRepository;

    public ActionService(ActionsRepository actionsRepository)
    {
        _actionsRepository = actionsRepository;
    }

#endregion
}