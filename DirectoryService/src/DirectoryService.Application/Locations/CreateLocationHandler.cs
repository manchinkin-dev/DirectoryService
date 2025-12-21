using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.Application.Locations;

public class CreateLocationHandler
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Handle(CreateLocationDto createLocationDto, CancellationToken cancellationToken)
    {
        var nameResult = LocationName.Create(createLocationDto.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }

        var address = new LocationAddress(
            createLocationDto.Address.Country,
            createLocationDto.Address.City,
            createLocationDto.Address.Street,
            createLocationDto.Address.HouseNumber,
            createLocationDto.Address.PostalCode);

        var timeZoneResult = TimeZone.Create(createLocationDto.TimeZone);

        if (timeZoneResult.IsFailure)
        {
            return timeZoneResult.Error;
        }

        var location = new Location(nameResult.Value, address, timeZoneResult.Value);

        await _locationsRepository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Локация создана с индентификатором - {locationId}.", location.Id.Value);

        return location.Id.Value;
    }
}