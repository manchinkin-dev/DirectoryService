using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(p => p.Request)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("request"));

        RuleFor(p => p.Request.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(p => p.Request.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(p => p.Request.DepartmentIds)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("список подразделений"));

        RuleFor(p => p.Request.DepartmentIds)
            .Must(departmentIds => departmentIds is { Length: > 0 })
            .WithError(
                GeneralErrors.Failure("Список подразделений должен содержать хотябы одну запись"));

        RuleFor(p => p.Request.DepartmentIds)
            .Must(departmentIds => departmentIds == null || departmentIds.Distinct().Count() == departmentIds.Length)
            .WithError(GeneralErrors.Failure("Список подразделений должен содержать уникальные значения"));
    }
}