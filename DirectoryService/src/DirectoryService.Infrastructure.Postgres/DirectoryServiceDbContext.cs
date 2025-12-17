using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure;

public class DirectoryServiceDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly bool _isDevelopment;

    public DirectoryServiceDbContext(string connectionString,  bool isDevelopment = false)
    {
        _connectionString = connectionString;
        _isDevelopment = isDevelopment;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(_connectionString);

        if (_isDevelopment)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();
}