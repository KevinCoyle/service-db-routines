using RoutinesDbService.Models;
using RoutinesDbService.Repositories;
using RoutinesDbService.Services.Interfaces;

namespace RoutinesDbService.Services.Implementations;

public class ScheduleService : IScheduleService
{
    public async Task<List<Schedule>> GetAllSchedules()
    {
        var results = await _schedulesRepository.GetAll();
        
        return results;
    }

    public async Task<Schedule> GetScheduleById(string id)
    {
        var result = await _schedulesRepository.GetById(id);
        
        return result;
    }

    public async Task<bool> CreateSchedule(Schedule schedule)
    {
        var result = await _schedulesRepository.Create(schedule);
        
        return result;
    }

    public async Task<bool> UpdateSchedule(Schedule schedule)
    {
        var result = await _schedulesRepository.Update(schedule);
        
        return result;
    }

    public async Task<bool> DeleteSchedule(string id)
    {
        var result = await _schedulesRepository.Delete(id);
        
        return result;
    }
    
    #region Constructor

    private readonly SchedulesRepository _schedulesRepository;

    public ScheduleService(SchedulesRepository schedulesRepository)
    {
        _schedulesRepository = schedulesRepository;
    }

    #endregion
}