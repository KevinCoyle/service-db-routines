using System.ComponentModel.DataAnnotations;

namespace Routines.Api.Domain;

public class User
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FullName { get; set; } = default!;

    public string Email { get; set; } = default!;
}
