using System.Net;
using Bogus;
using Routines.Api.Database;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Routines.Api.Contracts.Requests.Schedules;
using Routines.Api.Contracts.Responses.Schedules;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace Routines.Api.Tests.Integration;

public class ScheduleEndpointsTests : IAsyncLifetime
{
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly Faker<ScheduleRequest> _scheduleGenerator =
        new Faker<ScheduleRequest>()
            .RuleFor(x => x.Name, f => f.Random.Words(2))
            .RuleFor(x => x.Description, f => f.Random.Words(10))
            .UseSeed(1000);

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("routinesdb_local")
            .WithUsername("routinesdb_local")
            .WithPassword("P@ssw0rD")
            .Build();

    public ScheduleEndpointsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Create_ShouldCreateSchedule_WhenDetailsAreValid()
    {
        // Arrange
        var request = _scheduleGenerator.Generate();

        var expectedResponse = new ScheduleResponse
        {
            Name = request.Name,
            Description = request.Description,
        };

        // Act
        var response = await _client.PostAsJsonAsync("schedules", request);
        var scheduleResponse = await response.Content.ReadFromJsonAsync<ScheduleResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/schedules/{scheduleResponse!.Id}");
        scheduleResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        scheduleResponse.Id.Should().NotBeEmpty();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var waf = new WebApplicationFactory<IApiMarker>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(x =>
                {
                    x.ClearProviders();
                    x.SetMinimumLevel(LogLevel.Debug);
                    x.AddFilter(_ => true);
                    
                    x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(_testOutputHelper));
                });
                
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<RoutinesDbContext>>();
                    services.RemoveAll<RoutinesDbContext>();

                    services.AddDbContext<RoutinesDbContext>(x =>
                        x.UseNpgsql(_dbContainer.GetConnectionString()));
                });
            });
        
        _client = waf.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
