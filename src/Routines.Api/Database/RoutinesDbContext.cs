using Routines.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Action = Routines.Api.Domain.Action;

namespace Routines.Api.Database;

public class RoutinesDbContext : DbContext
{
    public DbSet<Action> Actions { get; set; } = null!;
    
    public DbSet<Routine> Routines { get; set; } = null!;
    
    public DbSet<Schedule> Schedules { get; set; } = null!;
    
    public DbSet<User> Users { get; set; } = null!;
    
    public RoutinesDbContext(DbContextOptions<RoutinesDbContext> options) : base(options)
    {
            
    }
}
