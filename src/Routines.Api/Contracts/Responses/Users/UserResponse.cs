﻿namespace Routines.Api.Contracts.Responses.Users;

public class UserResponse
{
    public Guid Id { get; init; }

    public string GitHubUsername { get; init; } = default!;

    public string FullName { get; init; } = default!;

    public string Email { get; init; } = default!;
}
