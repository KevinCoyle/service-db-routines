using Routines.Api.Contracts.Responses.Actions;
using Routines.Api.Contracts.Responses.Routines;
using Routines.Api.Contracts.Responses.Schedules;
using Routines.Api.Contracts.Responses.Users;
using Routines.Api.Domain;
using Action = Routines.Api.Domain.Action;

namespace Routines.Api.Mapping;

public static class DomainToApiContractMapper
{
    public static ActionResponse ToActionResponse(this Action action)
    {
        return new ActionResponse
        {
            Id = action.Id,
            Name = action.Name,
            Description = action.Description,
            RoutineId = action.RoutineId,
            Routine = action.Routine?.ToRoutineResponse(),
            FollowUpActionId = action.FollowUpActionId,
            FollowUpAction = action.FollowUpAction?.ToActionResponse(),
        };
    }
    
    public static GetAllActionsResponse ToActionsResponse(this IEnumerable<Action> actions)
    {
        return new GetAllActionsResponse
        {
            Actions = actions.Select(action => new ActionResponse
            {
                Id = action.Id,
                Name = action.Name,
                Description = action.Description,
                RoutineId = action.RoutineId,
                Routine = action.Routine?.ToRoutineResponse(),
                FollowUpActionId = action.FollowUpActionId,
                FollowUpAction = action.FollowUpAction?.ToActionResponse(),
            })
        };
    }
    
    public static RoutineResponse ToRoutineResponse(this Routine routine)
    {
        return new RoutineResponse
        {
            Id = routine.Id,
            Name = routine.Name,
            Description = routine.Description,
            FollowUpRoutine = routine.FollowUpRoutine?.ToRoutineResponse(),
            OwnerId = routine.OwnerId,
            Owner = routine.Owner?.ToUserResponse(),
            Actions = routine.Actions?.Select(ToActionResponse),
            Schedules = routine.Schedules?.Select(ToScheduleResponse),
        };
    }
    
    public static GetAllRoutinesResponse ToRoutinesResponse(this IEnumerable<Routine> routines)
    {
        return new GetAllRoutinesResponse
        {
            Routines = routines.Select(routine => new RoutineResponse()
            {
                Id = routine.Id,
                Name = routine.Name,
                Description = routine.Description,
                FollowUpRoutine = routine.FollowUpRoutine?.ToRoutineResponse(),
                OwnerId = routine.OwnerId,
                Owner = routine.Owner?.ToUserResponse(),
                Actions = routine.Actions?.Select(ToActionResponse),
                Schedules = routine.Schedules?.Select(ToScheduleResponse),
            })
        };
    }
    
    public static ScheduleResponse ToScheduleResponse(this Schedule schedule)
    {
        return new ScheduleResponse
        {
            Id = schedule.Id,
            Name = schedule.Name,
            Description = schedule.Description,
            Intervals = schedule.Intervals,
        };
    }

    public static GetAllSchedulesResponse ToSchedulesResponse(this IEnumerable<Schedule> schedules)
    {
        return new GetAllSchedulesResponse
        {
            Schedules = schedules.Select(schedule => new ScheduleResponse
            {
                Id = schedule.Id,
                Name = schedule.Name,
                Description = schedule.Description,
                Intervals = schedule.Intervals,
            })
        };
    }
    
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public static GetAllUsersResponse ToUsersResponse(this IEnumerable<User> users)
    {
        return new GetAllUsersResponse
        {
            Users = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            })
        };
    }
}
