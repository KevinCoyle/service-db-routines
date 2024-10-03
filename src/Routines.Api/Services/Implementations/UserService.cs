using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Routines.Api.Database;
using Routines.Api.Domain;
using Routines.Api.Services.Interfaces;

namespace Routines.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly RoutinesDbContext _dbContext;

    public UserService(RoutinesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(User user)
    {
        var existingUser = await _dbContext.Users.FindAsync(user.Id);
        if (existingUser is not null)
        {
            var message = $"A user with id {user.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(message));
        }

        await _dbContext.Users.AddAsync(user);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetAsync(id);
        if (user is null)
        {
            return false;
        }

        _dbContext.Remove(user);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    private static ValidationFailure[] GenerateValidationError(string message)
    {
        return new []
        {
            new ValidationFailure(nameof(User), message)
        };
    }
}
