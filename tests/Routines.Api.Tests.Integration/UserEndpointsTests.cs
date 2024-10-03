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

    private readonly Faker<UserRequest> _userGenerator =
        new Faker<UserRequest>()
            .RuleFor(x => x.FullName, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth.Date))
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
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth
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
