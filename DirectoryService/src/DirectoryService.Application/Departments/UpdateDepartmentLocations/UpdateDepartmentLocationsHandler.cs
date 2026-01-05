using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Fails;
using Shared.TransactionManager;

namespace DirectoryService.Application.Departments.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateDepartmentLocationsCommand> _validator;
    private readonly ILogger<UpdateDepartmentLocationsHandler> _logger;

    public UpdateDepartmentLocationsHandler(
        IDepartmentRepository departmentRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<UpdateDepartmentLocationsCommand> validator,
        ILogger<UpdateDepartmentLocationsHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error.ToErrors();

        using var transaction = transactionResult.Value;

        var departmentResult = await _departmentRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var existingLocationsResult =
            await _locationsRepository.CheckExistingAsync(command.Request.LocationIds, cancellationToken);
        if (existingLocationsResult.IsFailure)
            return existingLocationsResult.Error;

        var locationsDeleteResult =
            await _departmentRepository.DeleteLocationsAsync(command.DepartmentId, cancellationToken);
        if (locationsDeleteResult.IsFailure)
            return locationsDeleteResult.Error.ToErrors();

        var departmentLocations =
            command.Request.LocationIds.Select(lId =>
                new DepartmentLocation(new DepartmentId(command.DepartmentId), new LocationId(lId)));

        departmentResult.Value.UpdateLocations(departmentLocations);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        var commitedResult = transaction.Commit();
        if (commitedResult.IsFailure)
            return commitedResult.Error.ToErrors();

        _logger.LogInformation("Локации подразделения - {departmentId} успешно обновлены", command.DepartmentId);

        return command.DepartmentId;
    }
}