namespace Routines.Api.Contracts.Responses.Routines;

public class GetAllRoutinesResponse
{
    public IEnumerable<RoutineResponse> Routines { get; init; } = Enumerable.Empty<RoutineResponse>();
}
