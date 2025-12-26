using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;
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

        var addressResult = LocationAddress.Create(
            createLocationDto.Address.Country,
            createLocationDto.Address.City,
            createLocationDto.Address.Street,
            createLocationDto.Address.HouseNumber,
            createLocationDto.Address.PostalCode);

        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }

        var timeZoneResult = TimeZone.Create(createLocationDto.TimeZone);

        if (timeZoneResult.IsFailure)
        {
            return timeZoneResult.Error;
        }

        var location = new Location(nameResult.Value, addressResult.Value, timeZoneResult.Value);

        await _locationsRepository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Локация создана с индентификатором - {locationId}.", location.Id.Value);

        return location.Id.Value;
    }
}