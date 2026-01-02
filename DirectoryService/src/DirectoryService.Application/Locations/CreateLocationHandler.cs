using CSharpFunctionalExtensions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.Application.Locations;

public class CreateLocationHandler
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly IValidator<CreateLocationCommand> _validator;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        IValidator<CreateLocationCommand> validator,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand createLocationCommand,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(createLocationCommand, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var name = LocationName.Create(createLocationCommand.Request.Name).Value;

        var address = LocationAddress.Create(
            createLocationCommand.Request.Address.Country,
            createLocationCommand.Request.Address.City,
            createLocationCommand.Request.Address.Street,
            createLocationCommand.Request.Address.HouseNumber,
            createLocationCommand.Request.Address.PostalCode).Value;

        var timeZone = TimeZone.Create(createLocationCommand.Request.TimeZone).Value;

        var location = new Location(name, address, timeZone);

        var addLocationResult = await _locationsRepository.AddAsync(location, cancellationToken);

        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        _logger.LogInformation("Локация создана с индентификатором - {locationId}.", location.Id.Value);

        return location.Id.Value;
    }
}