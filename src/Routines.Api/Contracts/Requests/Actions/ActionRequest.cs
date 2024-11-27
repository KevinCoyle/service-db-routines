namespace Routines.Api.Contracts.Requests.Actions;

public class ActionRequest
{
    public Guid? Id { get; set; } = default!;

    public string? Name { get; init; } = default!;
}