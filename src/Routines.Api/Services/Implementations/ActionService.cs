using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Routines.Api.Database;
using Routines.Api.Services.Interfaces;
using Action = Routines.Api.Domain.Action;

namespace Routines.Api.Services.Implementations;

public class ActionService : IActionService
{
    private readonly RoutinesDbContext _dbContext;

    public ActionService(RoutinesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(Action action)
    {
        var existingAction = await _dbContext.Actions.FindAsync(action.Id);
        if (existingAction is not null)
        {
            var message = $"A action with id {action.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(message));
        }

        await _dbContext.Actions.AddAsync(action);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Action?> GetAsync(Guid id)
    {
        return await _dbContext.Actions.FindAsync(id);
    }

    public async Task<IEnumerable<Action>> GetAllAsync()
    {
        return await _dbContext.Actions.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Action action)
    {
        _dbContext.Actions.Update(action);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var action = await GetAsync(id);
        if (action is null)
        {
            return false;
        }

        _dbContext.Remove(action);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    private static ValidationFailure[] GenerateValidationError(string message)
    {
        return new []
        {
            new ValidationFailure(nameof(Action), message)
        };
    }
}
