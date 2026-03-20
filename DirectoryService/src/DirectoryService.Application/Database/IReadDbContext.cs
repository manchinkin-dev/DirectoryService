using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Location> LocationsRead { get; }

    IQueryable<DepartmentLocation> DepartmentLocationsRead { get; }
}