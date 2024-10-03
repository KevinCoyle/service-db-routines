using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Routines.Api.Database;
using Routines.Api.Services.Interfaces;
using Routine = Routines.Api.Domain.Routine;

namespace Routines.Api.Services.Implementations;

public class RoutineService : IRoutineService
{
    private readonly RoutinesDbContext _dbContext;

    public RoutineService(RoutinesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(Routine routine)
    {
        var existingRoutine = await _dbContext.Routines.FindAsync(routine.Id);
        if (existingRoutine is not null)
        {
            var message = $"A routine with id {routine.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(message));
        }

        await _dbContext.Routines.AddAsync(routine);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Routine?> GetAsync(Guid id)
    {
        return await _dbContext.Routines.FindAsync(id);
    }

    public async Task<IEnumerable<Routine>> GetAllAsync()
    {
        return await _dbContext.Routines.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Routine routine)
    {
        _dbContext.Routines.Update(routine);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var routine = await GetAsync(id);
        if (routine is null)
        {
            return false;
        }

        _dbContext.Remove(routine);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    private static ValidationFailure[] GenerateValidationError(string message)
    {
        return new []
        {
            new ValidationFailure(nameof(Routine), message)
        };
    }
}
