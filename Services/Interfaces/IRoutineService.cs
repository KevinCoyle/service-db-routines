using RoutinesDbService.Models;

namespace RoutinesDbService.Services.Interfaces;

public interface IRoutineService
{
    Task<List<Routine>> GetAllRoutines();
    Task<Routine> GetRoutineById(string id);
    Task<bool> CreateRoutine(Routine routine);
    Task<bool> UpdateRoutine(Routine routine);
    Task<bool> DeleteRoutine(string id);
}