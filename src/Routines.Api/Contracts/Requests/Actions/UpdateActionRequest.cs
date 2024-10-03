using Microsoft.AspNetCore.Mvc;

namespace Routines.Api.Contracts.Requests.Actions;

public class UpdateActionRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public ActionRequest Action { get; set; } = default!;
}