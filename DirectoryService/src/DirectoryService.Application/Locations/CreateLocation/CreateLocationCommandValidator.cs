using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(x => x.Request.Address)
            .MustBeValueObject(dto =>
                LocationAddress.Create(
                    dto.Country,
                    dto.City,
                    dto.Street,
                    dto.HouseNumber,
                    dto.PostalCode));

        RuleFor(x => x.Request.TimeZone)
            .MustBeValueObject(TimeZone.Create);
    }
}