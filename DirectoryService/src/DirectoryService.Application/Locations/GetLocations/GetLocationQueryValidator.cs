using DirectoryService.Application.Validation;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Locations.GetLocations;

public class GetLocationQueryValidator : AbstractValidator<GetLocationQuery>
{
    public GetLocationQueryValidator()
    {
        RuleFor(x => x.Request.Page)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("Page"));

        RuleFor(x => x.Request.PageSize)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("PageSize"));
    }
}