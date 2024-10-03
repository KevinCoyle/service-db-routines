using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Routines.Api.Database;
using Routines.Api.Services.Interfaces;
using Schedule = Routines.Api.Domain.Schedule;

namespace Routines.Api.Services.Implementations;

public class ScheduleService : IScheduleService
{
    private readonly RoutinesDbContext _dbContext;

    public ScheduleService(RoutinesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(Schedule schedule)
    {
        var existingSchedule = await _dbContext.Schedules.FindAsync(schedule.Id);
        if (existingSchedule is not null)
        {
            var message = $"A schedule with id {schedule.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(message));
        }

        await _dbContext.Schedules.AddAsync(schedule);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Schedule?> GetAsync(Guid id)
    {
        return await _dbContext.Schedules.FindAsync(id);
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _dbContext.Schedules.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Schedule schedule)
    {
        _dbContext.Schedules.Update(schedule);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await GetAsync(id);
        if (schedule is null)
        {
            return false;
        }

        _dbContext.Remove(schedule);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    private static ValidationFailure[] GenerateValidationError(string message)
    {
        return new []
        {
            new ValidationFailure(nameof(Schedule), message)
        };
    }
}
