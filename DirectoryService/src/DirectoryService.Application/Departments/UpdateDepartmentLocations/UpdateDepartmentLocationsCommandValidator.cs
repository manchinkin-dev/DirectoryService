using DirectoryService.Application.Validation;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Departments.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
        RuleFor(dl => dl.Request)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("request"));

        RuleFor(dl => dl.DepartmentId)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("идентификатор подразделения"));

        RuleFor(dl => dl.Request.LocationIds)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("список локаций"));

        RuleFor(dl => dl.Request.LocationIds)
            .Must(ids => ids is { Length: > 0 })
            .WithError(GeneralErrors.Failure("список локаций должен содержать хотябы одну запись"));

        RuleFor(dl => dl.Request.LocationIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Length)
            .WithError(GeneralErrors.Failure("Список локаций должен содержать уникальные значения"));
    }
}