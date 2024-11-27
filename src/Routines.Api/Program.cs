using Routines.Api.Database;
using Routines.Api.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Routines.Api.Services.Implementations;
using Routines.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

var config = builder.Configuration;
config.AddEnvironmentVariables("UsersApi_");

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation(c => { c.DisableDataAnnotationsValidation = true; });
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RoutinesDbContext>(x => x.UseNpgsql("Database:ConnectionString"));
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.MapControllers();

using var migrationScope = app.Services.CreateScope();
var db = migrationScope.ServiceProvider.GetRequiredService<RoutinesDbContext>();
await db.Database.MigrateAsync();

app.Run();
