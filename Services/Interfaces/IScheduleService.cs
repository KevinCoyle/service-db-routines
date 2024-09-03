using RoutinesDbService.Models;

namespace RoutinesDbService.Services.Interfaces;

public interface IScheduleService
{
    Task<List<Schedule>> GetAllSchedules();
    Task<Schedule> GetScheduleById(string id);
    Task<bool> CreateSchedule(Schedule schedule);
    Task<bool> UpdateSchedule(Schedule schedule);
    Task<bool> DeleteSchedule(string id);
}