using DirectoryService.Application.Database;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure;

public class DirectoryServiceDbContext : DbContext, IReadDbContext
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

        var dataSource = new NpgsqlDataSourceBuilder(_connectionString)
            .EnableDynamicJson()
            .Build();

        optionsBuilder.UseNpgsql(dataSource);

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
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();

    public IQueryable<Location> LocationsRead =>
        Set<Location>()
            .AsNoTracking()
            .AsQueryable();

    public IQueryable<DepartmentLocation> DepartmentLocationsRead =>
        Set<DepartmentLocation>()
            .AsNoTracking()
            .AsQueryable();

    public IQueryable<Department> DepartmentsRead =>
        Set<Department>()
            .AsNoTracking()
            .AsQueryable();

    public IQueryable<DepartmentPosition> DepartmentPositionsRead =>
        Set<DepartmentPosition>()
            .AsNoTracking()
            .AsQueryable();
}