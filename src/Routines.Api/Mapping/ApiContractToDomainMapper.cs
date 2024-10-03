using Routines.Api.Contracts.Requests.Actions;
using Routines.Api.Contracts.Requests.Routines;
using Routines.Api.Contracts.Requests.Schedules;
using Routines.Api.Contracts.Requests.Users;
using Routines.Api.Domain;
using Action = Routines.Api.Domain.Action;

namespace Routines.Api.Mapping;

public static class ApiContractToDomainMapper
{
    public static Action ToAction(this ActionRequest request)
    {
        return new Action
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };
    }
    
    public static Action ToAction(this UpdateActionRequest request)
    {
        return new Action
        {
            Id = request.Id,
            Name = request.Action.Name,
        };
    }
    
    public static Routine ToRoutine(this RoutineRequest request)
    {
        return new Routine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = request.OwnerId,
        };
    }
    
    public static Routine ToRoutine(this UpdateRoutineRequest request)
    {
        return new Routine
        {
            Id = request.Id,
            Name = request.Routine.Name,
            OwnerId = request.Routine.OwnerId,
        };
    }
    
    public static Schedule ToSchedule(this ScheduleRequest request)
    {
        return new Schedule
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
        };
    }
    
    public static Schedule ToSchedule(this UpdateScheduleRequest request)
    {
        return new Schedule
        {
            Id = request.Id,
            Name = request.Schedule.Name,
            Description = request.Schedule.Description,
        };
    }
    
    public static User ToUser(this UserRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth
        };
    }

    public static User ToUser(this UpdateUserRequest request)
    {
        return new User
        {
            Id = request.Id,
            Email = request.User.Email,
            FullName = request.User.FullName,
            DateOfBirth = request.User.DateOfBirth
        };
    }
}
