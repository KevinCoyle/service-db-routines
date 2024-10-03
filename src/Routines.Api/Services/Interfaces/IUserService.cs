using Routines.Api.Domain;

namespace Routines.Api.Services.Interfaces;

public interface IUserService
{
    Task<bool> CreateAsync(User user);

    Task<User?> GetAsync(Guid id);

    Task<IEnumerable<User>> GetAllAsync();

    Task<bool> UpdateAsync(User user);

    Task<bool> DeleteAsync(Guid id);
}
