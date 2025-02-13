using System.Net;
using Bogus;
using Routines.Api.Database;
using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Routines.Api.Contracts.Requests.Users;
using Routines.Api.Contracts.Responses.Users;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace Routines.Api.Tests.Integration;

public class UserEndpointsTests : IAsyncLifetime
{
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _testOutputHelper;
    private RoutinesDbContext? _dbContext;

    private readonly Faker<UserRequest> _userGenerator =
        new Faker<UserRequest>()
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .UseSeed(1000);

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("routinesdb_local")
            .WithUsername("routinesdb_local")
            .WithPassword("P@ssw0rD")
            .Build();

    public UserEndpointsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenDetailsAreValid()
    {
        // Arrange
        var request = _userGenerator.Generate();

        var expectedResponse = new UserResponse
        {
            Email = request.Email,
            FullName = request.FullName
        };

        // Act
        var response = await _client.PostAsJsonAsync("users", request);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"http://localhost/users/{userResponse!.Id}");
        userResponse.Should().BeEquivalentTo(expectedResponse, opt => opt.Excluding(x => x.Id));
        userResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldGetAllUsers_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponseCount = _dbContext?.Users.ToList().Count;

        // Act
        var response = await _client.GetAsync($"users");
        var actualResponse = await response.Content.ReadFromJsonAsync<GetAllUsersResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Users.Should().NotBeNullOrEmpty();
        actualResponse.Users.Count().Should().Be(expectedResponseCount);
    }

    [Fact]
    public async Task Get_ShouldGetUser_WhenDetailsAreValid()
    {
        // Arrange
        var expectedResponse = await _dbContext?.Users.FirstAsync()!;

        // Act
        var response = await _client.GetAsync($"users/{expectedResponse.Id}");
        var actualResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldUpdateUser_WhenDetailsAreValid()
    {
        // Arrange
        var request = _userGenerator.Generate();
        var requestId = (await _dbContext?.Users.FirstAsync()!).Id;
        var expectedResponse = request;
        expectedResponse.Id = requestId;

        // Act
        var response = await _client.PutAsJsonAsync($"users/{requestId}", request);
        var actualResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actualResponse!.Id.Should().Be(expectedResponse.Id ?? Guid.Empty);
        actualResponse!.FullName.Should().Be(expectedResponse.FullName);
    }

    [Fact]
    public async Task Delete_ShouldDeleteUser_WhenDetailsAreValid()
    {
        // Arrange
        var request = (await _dbContext?.Users.FirstAsync()!).Id;
        var countBeforeDelete = _dbContext?.Users.ToList().Count;

        // Act
        var response = await _client.DeleteAsync($"users/{request}");
        var countAfterDelete = _dbContext?.Users.ToList().Count;

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
                                var faker = new Faker<Routines.Api.Domain.User>()
                                    .RuleFor(x => x.Id, f => f.Random.Guid())
                                    .RuleFor(x => x.FullName, f => f.Person.FullName)
                                    .RuleFor(x => x.Email, f => f.Person.Email)
                                    .UseSeed(420);
                    
                                var usersToSeed = faker.Generate(10);
                                
                                var contains = await context.Set<Routines.Api.Domain.User>().ContainsAsync(usersToSeed[0], cancellationToken: ct);

                                if (!contains)
                                {
                                    await context.Set<Routines.Api.Domain.User>().AddRangeAsync(usersToSeed, cancellationToken: ct);
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
