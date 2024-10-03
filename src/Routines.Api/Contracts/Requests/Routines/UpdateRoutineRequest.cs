using Microsoft.AspNetCore.Mvc;

namespace Routines.Api.Contracts.Requests.Routines;

public class UpdateRoutineRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public RoutineRequest Routine { get; set; } = default!;
}