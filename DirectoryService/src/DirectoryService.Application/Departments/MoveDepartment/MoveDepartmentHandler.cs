using CSharpFunctionalExtensions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Fails;
using Shared.TransactionManager;

namespace DirectoryService.Application.Departments.MoveDepartment;

public class MoveDepartmentHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<MoveDepartmentCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<MoveDepartmentHandler> _logger;

    public MoveDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IValidator<MoveDepartmentCommand> validator,
        ITransactionManager transactionManager,
        ILogger<MoveDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _validator = validator;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        MoveDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);

        using var connection = transaction.Value;

        var departmentResult = await _departmentRepository.GetByIdWithLocks(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var department = departmentResult.Value;

        DepartmentId? parentId =
            command.Request.ParentId.HasValue
                ? new DepartmentId(command.Request.ParentId.Value)
                : null;

        if (parentId != null && command.DepartmentId == parentId.Value)
            return GeneralErrors.ValueIsInvalid("идентификатор подразделения").ToErrors();

        Department? parentDepartment = null;

        if (parentId?.Value != null)
        {
            var parentResult = await _departmentRepository.GetByIdWithLocks(parentId.Value, cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Error.ToErrors();

            parentDepartment = parentResult.Value;

            if (department.Path == parentDepartment.Path || parentDepartment.Path.Value.StartsWith(department.Path.Value))
            {
                GeneralErrors.Failure("Нельзя подразделение перенести к своему потомку");
            }
        }

        var oldPath = department.Path;
        int depth = department.MoveTo(parentDepartment);

        var lockDescendantsResult = await _departmentRepository.LockDescendantsByPath(oldPath.Value, cancellationToken);
        if (lockDescendantsResult.IsFailure)
            return lockDescendantsResult.Error.ToErrors();

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        var updateResult = await _departmentRepository.BulkUpdateDescendantsPathAndDepth(oldPath.Value, department.Path.Value, depth, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult.Error.ToErrors();

        var result = connection.Commit();
        if (result.IsFailure)
            return result.Error.ToErrors();

        _logger.LogInformation("Подразделение с идентификатором - {departmentId} успешно перенесено!", command.DepartmentId);

        return command.DepartmentId;
    }
}