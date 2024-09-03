using RoutinesDbService.Models;
using RoutinesDbService.Repositories;
using RoutinesDbService.Services.Implementations;
using RoutinesDbService.Services.Interfaces;

namespace RoutinesDbService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<ActionsRepository>();
        builder.Services.AddSingleton<RoutinesRepository>();
        builder.Services.AddSingleton<SchedulesRepository>();
        builder.Services.AddSingleton<UsersRepository>();
        builder.Services.AddTransient<IActionService, ActionService>();
        builder.Services.AddTransient<IRoutineService, RoutineService>();
        builder.Services.AddTransient<IScheduleService, ScheduleService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.Configure<ConnectionString>(builder.Configuration.GetSection("ConnectionStrings"));
        var app = builder.Build();

        app.Run();
    }
}