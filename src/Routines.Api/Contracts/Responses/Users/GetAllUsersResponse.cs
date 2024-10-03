namespace Routines.Api.Contracts.Responses.Users;

public class GetAllUsersResponse
{
    public IEnumerable<UserResponse> Users { get; init; } = Enumerable.Empty<UserResponse>();
}
