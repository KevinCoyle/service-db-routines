namespace Routines.Api.Contracts.Responses.Actions;

public class GetAllActionsResponse
{
    public IEnumerable<ActionResponse> Actions { get; init; } = Enumerable.Empty<ActionResponse>();
}
