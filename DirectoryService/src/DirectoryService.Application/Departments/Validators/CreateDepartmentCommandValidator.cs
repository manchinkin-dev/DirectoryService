using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Shared.Errors;

namespace DirectoryService.Application.Departments.Validators;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(d => d.Request)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("request"));

        RuleFor(d => d.Request.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(d => d.Request.Identifier)
            .MustBeValueObject(Identifier.Create);

        RuleFor(d => d.Request.LocationIds)
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsRequired("список локаций"));

        RuleFor(d => d.Request.LocationIds)
            .Must(locationIds => locationIds is { Length: > 0 })
            .WithError(GeneralErrors.Failure("Список локаций должен содержать хотябы одну запись"));

        RuleFor(d => d.Request.LocationIds)
            .Must(locationIds => locationIds == null || locationIds.Distinct().Count() == locationIds.Length)
            .WithError(GeneralErrors.Failure(
                "Список локаций должен содержать уникальные значения"));
    }
}