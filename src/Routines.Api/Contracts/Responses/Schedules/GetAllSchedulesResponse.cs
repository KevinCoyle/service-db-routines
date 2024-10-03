namespace Routines.Api.Contracts.Responses.Schedules;

public class GetAllSchedulesResponse
{
    public IEnumerable<ScheduleResponse> Schedules { get; init; } = Enumerable.Empty<ScheduleResponse>();
}
