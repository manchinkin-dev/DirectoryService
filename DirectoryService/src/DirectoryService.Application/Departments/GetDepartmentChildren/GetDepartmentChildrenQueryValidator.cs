using DirectoryService.Application.Validation;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Departments.GetDepartmentChildren;

public class GetDepartmentChildrenQueryValidator : AbstractValidator<GetDepartmentChildrenQuery>
{
    public GetDepartmentChildrenQueryValidator()
    {
        RuleFor(x => x.ParentId)
            .NotNull()
            .WithError(GeneralErrors.ValueIsRequired("ParentId"));
    }
}