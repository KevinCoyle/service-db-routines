using Routines.Api.Domain;

namespace Routines.Api.Services.Interfaces;

public interface IScheduleService
{
    Task<bool> CreateAsync(Schedule schedule);

    Task<Schedule?> GetAsync(Guid id);

    Task<IEnumerable<Schedule>> GetAllAsync();

    Task<bool> UpdateAsync(Schedule schedule);

    Task<bool> DeleteAsync(Guid id);
}
