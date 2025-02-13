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
    private RoutinesDbContext? _dbContext;

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
        var actualResponse = await response.Content.ReadFromJsonAsync<ScheduleResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/schedules/{actualResponse!.Id}");
        actualResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        actualResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldGetAllSchedules_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponseCount = _dbContext?.Schedules.ToList().Count;

        // Act
        var response = await _client.GetAsync($"schedules");
        var actualResponse = await response.Content.ReadFromJsonAsync<GetAllSchedulesResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Schedules.Should().NotBeNullOrEmpty();
        actualResponse.Schedules.Count().Should().Be(expectedResponseCount);
    }

    [Fact]
    public async Task Get_ShouldGetSchedule_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponse = await _dbContext?.Schedules.FirstAsync()!;

        // Act
        var response = await _client.GetAsync($"schedules/{expectedResponse.Id}");
        var actualResponse = await response.Content.ReadFromJsonAsync<ScheduleResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldUpdateSchedule_WhenDetailsAreValid()
    {
        // Arrange
        var request = _scheduleGenerator.Generate();
        var requestId = (await _dbContext?.Schedules.FirstAsync()!).Id;
        var expectedResponse = request;
        expectedResponse.Id = requestId;

        // Act
        var response = await _client.PutAsJsonAsync($"schedules/{requestId}", request);
        var actualResponse = await response.Content.ReadFromJsonAsync<ScheduleResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Id.Should().Be(expectedResponse.Id ?? Guid.Empty);
        actualResponse!.Name.Should().Be(expectedResponse.Name);
    }

    [Fact]
    public async Task Delete_ShouldDeleteSchedule_WhenDetailsAreValid()
    {
        // Arrange
        var request = (await _dbContext?.Schedules.FirstAsync()!).Id;
        var countBeforeDelete = _dbContext?.Schedules.ToList().Count;

        // Act
        var response = await _client.DeleteAsync($"schedules/{request}");
        var countAfterDelete = _dbContext?.Schedules.ToList().Count;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        countAfterDelete.Should().Be(countBeforeDelete - 1);
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

                    services.AddDbContext<RoutinesDbContext>(optionsBuilder =>
                        optionsBuilder.UseNpgsql(_dbContainer.GetConnectionString())
                            .UseAsyncSeeding(async (context, _, ct) =>
                            {
                                var faker = new Faker<Routines.Api.Domain.Schedule>()
                                    .RuleFor(x => x.Id, f => f.Random.Guid())
                                    .RuleFor(x => x.Name, f => f.Random.Words(3))
                                    .RuleFor(x => x.Description, f => f.Random.Words(8))
                                    .UseSeed(420);
                                
                                var schedulesToSeed = faker.Generate(10);
                                
                                var contains = await context.Set<Routines.Api.Domain.Schedule>().ContainsAsync(schedulesToSeed[0], cancellationToken: ct);

                                if (!contains)
                                {
                                    await context.Set<Routines.Api.Domain.Schedule>().AddRangeAsync(schedulesToSeed, cancellationToken: ct);
                                    await context.SaveChangesAsync();
                                }
                            })
                    );
                    
                    _dbContext = services.BuildServiceProvider().GetService<RoutinesDbContext>()!;
                });
            });
        
        _client = waf.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
