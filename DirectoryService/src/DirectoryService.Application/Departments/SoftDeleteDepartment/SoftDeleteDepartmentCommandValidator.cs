using DirectoryService.Application.Validation;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Departments.SoftDeleteDepartment;

public class SoftDeleteDepartmentCommandValidator : AbstractValidator<SoftDeleteDepartmentCommand>
{
    public SoftDeleteDepartmentCommandValidator()
    {
        RuleFor(c => c.DepartmentId)
            .NotEmpty()
            .WithError(GeneralErrors.ValueIsRequired("DepartmentId"));
    }
}