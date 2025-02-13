namespace Routines.Api.Contracts.Requests.Users;

public class UserRequest
{
    public Guid? Id { get; set; } = default!;
    
    public string? FullName { get; init; } = default!;

    public string? Email { get; init; } = default!;
}
