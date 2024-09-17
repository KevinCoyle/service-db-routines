using RoutinesDbService.Models;
using RoutinesDbService.Repositories;
using RoutinesDbService.Services.Interfaces;

namespace RoutinesDbService.Services.Implementations;

public class RoutineService : IRoutineService
{
    public async Task<List<Routine>> GetAllRoutines()
    {
        var results = await _routinesRepository.GetAll();
        
        return results;
    }

    public async Task<Routine?> GetRoutineById(string id)
    {
        var result = await _routinesRepository.GetById(id);
        
        return result;
    }

    public async Task<bool> CreateRoutine(Routine routine)
    {
        var result = await _routinesRepository.Create(routine);
        
        return result;
    }

    public async Task<bool> UpdateRoutine(Routine routine)
    {
        var result = await _routinesRepository.Update(routine);
        
        return result;
    }

    public async Task<bool> DeleteRoutine(string id)
    {
        var result = await _routinesRepository.Delete(id);
        
        return result;
    }
    
#region Constructor

    private readonly RoutinesRepository _routinesRepository;

    public RoutineService(RoutinesRepository routinesRepository)
    {
        _routinesRepository = routinesRepository;
    }

#endregion
}