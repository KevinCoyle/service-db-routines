using System.Net;
using Bogus;
using Routines.Api.Database;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Routines.Api.Contracts.Requests.Routines;
using Routines.Api.Contracts.Responses.Routines;
using Routines.Api.Mapping;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace Routines.Api.Tests.Integration;

public class RoutineEndpointsTests : IAsyncLifetime
{
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _testOutputHelper;
    private RoutinesDbContext? _dbContext;

    private readonly Faker<RoutineRequest> _routineGenerator =
        new Faker<RoutineRequest>()
            .RuleFor(x => x.Name, f => f.Random.Words(2))
            .UseSeed(1000);

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("routinesdb_local")
            .WithUsername("routinesdb_local")
            .WithPassword("P@ssw0rD")
            .Build();

    public RoutineEndpointsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Create_ShouldCreateRoutine_WhenDetailsAreValid()
    {
        // Arrange
        var request = _routineGenerator.Generate();

        var expectedResponse = new RoutineResponse
        {
            Name = request.Name,
        };

        // Act
        var response = await _client.PostAsJsonAsync("routines", request);
        var routineResponse = await response.Content.ReadFromJsonAsync<RoutineResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/routines/{routineResponse!.Id}");
        routineResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        routineResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldGetAllRoutines_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponseCount = _dbContext?.Routines.ToList().Count;

        // Act
        var response = await _client.GetAsync($"routines");
        var actualResponse = await response.Content.ReadFromJsonAsync<GetAllRoutinesResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Routines.Should().NotBeNullOrEmpty();
        actualResponse.Routines.Count().Should().Be(expectedResponseCount);
    }

    [Fact]
    public async Task Get_ShouldGetRoutine_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponse = await _dbContext?.Routines.FirstAsync()!;

        // Act
        var response = await _client.GetAsync($"routines/{expectedResponse.Id}");
        var actualResponse = await response.Content.ReadFromJsonAsync<RoutineResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse.Should().BeEquivalentTo(expectedResponse.ToRoutineResponse());
    }

    [Fact]
    public async Task Update_ShouldUpdateRoutine_WhenDetailsAreValid()
    {
        // Arrange
        var request = _routineGenerator.Generate();
        var requestId = (await _dbContext?.Routines.FirstAsync()!).Id;
        var expectedResponse = request;
        expectedResponse.Id = requestId;

        // Act
        var response = await _client.PutAsJsonAsync($"routines/{requestId}", request);
        var actualResponse = await response.Content.ReadFromJsonAsync<RoutineResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Id.Should().Be(expectedResponse.Id ?? Guid.Empty);
        actualResponse!.Name.Should().Be(expectedResponse.Name);
    }

    [Fact]
    public async Task Delete_ShouldDeleteRoutine_WhenDetailsAreValid()
    {
        // Arrange
        var request = (await _dbContext?.Routines.FirstAsync()!).Id;
        var countBeforeDelete = _dbContext?.Routines.ToList().Count;

        // Act
        var response = await _client.DeleteAsync($"routines/{request}");
        var countAfterDelete = _dbContext?.Routines.ToList().Count;

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
                                var faker = new Faker<Routines.Api.Domain.Routine>()
                                    .RuleFor(x => x.Id, f => f.Random.Guid())
                                    .RuleFor(x => x.Name, f => f.Random.Words(3))
                                    .RuleFor(x => x.Description, f => f.Random.Words(8))
                                    .UseSeed(420);
                                
                                var routinesToSeed = faker.Generate(10);
                                
                                var contains = await context.Set<Routines.Api.Domain.Routine>().ContainsAsync(routinesToSeed[0], cancellationToken: ct);

                                if (!contains)
                                {
                                    await context.Set<Routines.Api.Domain.Routine>().AddRangeAsync(routinesToSeed, cancellationToken: ct);
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
