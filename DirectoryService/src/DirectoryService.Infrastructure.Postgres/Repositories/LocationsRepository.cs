using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Fails;

namespace DirectoryService.Infrastructure.Repositories;

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
            await _dbContext.Locations.AddAsync(location, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка добавления локации");

            return Error.Failure("location.insert", "Ошибка добавления локации");
        }
    }

    public async Task<UnitResult<Errors>> CheckExistingAsync(
        Guid[] locationIds,
        CancellationToken cancellationToken = default)
    {
        var ids = locationIds.Select(id => new LocationId(id));

        var resultIds = await _dbContext.Locations
            .AsNoTracking()
            .Where(l => ids.Contains(l.Id) && l.IsActive)
            .Select(l => l.Id.Value)
            .ToListAsync(cancellationToken);

        var missingIds = locationIds.Except(resultIds);

        var errors = missingIds.Select(mi => GeneralErrors.NotFound(mi, "идентификатор локации")).ToList();

        return errors.Count != 0
            ? UnitResult.Failure(new Errors(errors))
            : UnitResult.Success<Errors>();
    }
}