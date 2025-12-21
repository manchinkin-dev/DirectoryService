using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<LocationsRepository> _logger;

    public LocationsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<LocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Locations.Add(location);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка добавления локации");

            return Error.Failure("location.insert", "Ошибка добавления локации");
        }
    }
}