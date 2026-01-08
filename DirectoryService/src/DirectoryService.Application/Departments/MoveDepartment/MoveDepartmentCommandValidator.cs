using DirectoryService.Application.Validation;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Departments.MoveDepartment;

public class MoveDepartmentCommandValidator : AbstractValidator<MoveDepartmentCommand>
{
    public MoveDepartmentCommandValidator()
    {
        RuleFor(d => d.DepartmentId)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("departmentId"));
    }
}