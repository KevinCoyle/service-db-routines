using Microsoft.AspNetCore.Mvc;

namespace Routines.Api.Contracts.Requests.Schedules;

public class UpdateScheduleRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public ScheduleRequest Schedule { get; set; } = default!;
}