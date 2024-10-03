using Routines.Api.Domain;

namespace Routines.Api.Services.Interfaces;

public interface IRoutineService
{
    Task<bool> CreateAsync(Routine routine);

    Task<Routine?> GetAsync(Guid id);

    Task<IEnumerable<Routine>> GetAllAsync();

    Task<bool> UpdateAsync(Routine routine);

    Task<bool> DeleteAsync(Guid id);
}
