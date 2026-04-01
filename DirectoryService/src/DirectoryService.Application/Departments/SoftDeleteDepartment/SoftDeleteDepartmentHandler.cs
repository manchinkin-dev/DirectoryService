using CSharpFunctionalExtensions;
using DirectoryService.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Fails;
using Shared.TransactionManager;

namespace DirectoryService.Application.Departments.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<SoftDeleteDepartmentCommand> _validator;
    private readonly ILogger<SoftDeleteDepartmentHandler> _logger;

    public SoftDeleteDepartmentHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager,
        IValidator<SoftDeleteDepartmentCommand> validator,
        ILogger<SoftDeleteDepartmentHandler> logger)
    {
        _repository = repository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        SoftDeleteDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();

        using var transaction = transactionResult.Value;

        var departmentResult = await _repository.GetByIdWithLocks(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var department = departmentResult.Value;
        if (!department.IsActive)
            return GeneralErrors.ValueIsInvalid("IsActive").ToErrors();

        string oldPath = department.Path.Value;

        department.SoftDelete();

        string newPath = department.Path.Value;

        var lockDescendantsResult = await _repository.LockDescendantsByPath(oldPath, cancellationToken);
        if (lockDescendantsResult.IsFailure)
            return lockDescendantsResult.Error.ToErrors();

        var bulkUpdateDescendantsPath =
            await _repository.BulkUpdateDescendantsPath(oldPath, newPath, cancellationToken);
        if (bulkUpdateDescendantsPath.IsFailure)
            return bulkUpdateDescendantsPath.Error.ToErrors();

        var deactivateLocationsOrPositionsResult =
            await _repository.DeactivateLocationsOrPositions(department.Id, cancellationToken);
        if (deactivateLocationsOrPositionsResult.IsFailure)
            return deactivateLocationsOrPositionsResult.Error.ToErrors();

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();

        _logger.LogInformation("Подразделение с идентификатором - {CommandDepartmentId} был успешно удален", command.DepartmentId);

        return UnitResult.Success<Errors>();
    }
}