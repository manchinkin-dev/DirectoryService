using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken = default);
}