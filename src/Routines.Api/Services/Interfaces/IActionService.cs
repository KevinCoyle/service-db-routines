using Action = Routines.Api.Domain.Action;

namespace Routines.Api.Services.Interfaces;

public interface IActionService
{
    Task<bool> CreateAsync(Action action);

    Task<Action?> GetAsync(Guid id);

    Task<IEnumerable<Action>> GetAllAsync();

    Task<bool> UpdateAsync(Action action);

    Task<bool> DeleteAsync(Guid id);
}
