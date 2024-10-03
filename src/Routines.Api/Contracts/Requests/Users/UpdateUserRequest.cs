using Microsoft.AspNetCore.Mvc;

namespace Routines.Api.Contracts.Requests.Users;

public class UpdateUserRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public UserRequest User { get; set; } = default!;
}
