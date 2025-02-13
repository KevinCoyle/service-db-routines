using System.Net;
using Bogus;
using Routines.Api.Database;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Routines.Api.Contracts.Requests.Actions;
using Routines.Api.Contracts.Responses.Actions;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace Routines.Api.Tests.Integration;

public class ActionEndpointsTests : IAsyncLifetime
{
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _testOutputHelper;
    private RoutinesDbContext? _dbContext;

    private readonly Faker<ActionRequest> _actionGenerator =
        new Faker<ActionRequest>()
            .RuleFor(x => x.Name, f => f.Random.Words(2))
            .UseSeed(1000);

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("routinesdb_local")
            .WithUsername("routinesdb_local")
            .WithPassword("P@ssw0rD")
            .Build();

    public ActionEndpointsTests(
        ITestOutputHelper testOutputHelper
    )
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Create_ShouldCreateAction_WhenDetailsAreValid()
    {
        // Arrange
        var request = _actionGenerator.Generate();

        var expectedResponse = new ActionResponse
        {
            Name = request.Name,
        };

        // Act
        var response = await _client.PostAsJsonAsync("actions", request);
        var actualResponse = await response.Content.ReadFromJsonAsync<ActionResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/actions/{actualResponse!.Id}");
        actualResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        actualResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldGetAllActions_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponseCount = _dbContext?.Actions.ToList().Count;

        // Act
        var response = await _client.GetAsync($"actions");
        var actualResponse = await response.Content.ReadFromJsonAsync<GetAllActionsResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Actions.Should().NotBeNullOrEmpty();
        actualResponse.Actions.Count().Should().Be(expectedResponseCount);
    }

    [Fact]
    public async Task Get_ShouldGetAction_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponse = await _dbContext?.Actions.FirstAsync()!;

        // Act
        var response = await _client.GetAsync($"actions/{expectedResponse.Id}");
        var actualResponse = await response.Content.ReadFromJsonAsync<ActionResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldUpdateAction_WhenDetailsAreValid()
    {
        // Arrange
        var request = _actionGenerator.Generate();
        var requestId = (await _dbContext?.Actions.FirstAsync()!).Id;
        var expectedResponse = request;
        expectedResponse.Id = requestId;

        // Act
        var response = await _client.PutAsJsonAsync($"actions/{requestId}", request);
        var actualResponse = await response.Content.ReadFromJsonAsync<ActionResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Id.Should().Be(expectedResponse.Id ?? Guid.Empty);
        actualResponse!.Name.Should().Be(expectedResponse.Name);
    }

    [Fact]
    public async Task Delete_ShouldDeleteAction_WhenDetailsAreValid()
    {
        // Arrange
        var request = (await _dbContext?.Actions.FirstAsync()!).Id;
        var countBeforeDelete = _dbContext?.Actions.ToList().Count;

        // Act
        var response = await _client.DeleteAsync($"actions/{request}");
        var countAfterDelete = _dbContext?.Actions.ToList().Count;

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
                                var faker = new Faker<Routines.Api.Domain.Action>()
                                    .RuleFor(x => x.Id, f => f.Random.Guid())
                                    .RuleFor(x => x.Name, f => f.Random.Words(3))
                                    .RuleFor(x => x.Description, f => f.Random.Words(8))
                                    .UseSeed(420);
                                
                                var actionsToSeed = faker.Generate(10);
                                
                                var contains = await context.Set<Routines.Api.Domain.Action>().ContainsAsync(actionsToSeed[0], cancellationToken: ct);

                                if (!contains)
                                {
                                    await context.Set<Routines.Api.Domain.Action>().AddRangeAsync(actionsToSeed, cancellationToken: ct);
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
