using System.ComponentModel.DataAnnotations;

namespace Routines.Api.Domain;

public class User
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FullName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public DateOnly DateOfBirth { get; init; } = default!;
}
