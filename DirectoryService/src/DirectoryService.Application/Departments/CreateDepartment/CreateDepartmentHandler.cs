using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Fails;

namespace DirectoryService.Application.Departments.CreateDepartment;

public class CreateDepartmentHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationsRepository locationsRepository,
        IValidator<CreateDepartmentCommand> validator,
        ILogger<CreateDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var departmentId = new DepartmentId(Guid.NewGuid());
        var name = DepartmentName.Create(command.Request.Name).Value;
        var identifier = Identifier.Create(command.Request.Identifier).Value;

        var parentId = command.Request.ParentId;

        var checkExistingResult =
            await _locationsRepository.CheckExistingAsync(command.Request.LocationIds, cancellationToken);

        if (checkExistingResult.IsFailure)
        {
            return checkExistingResult.Error;
        }

        Department? parent = null;

        if (parentId.HasValue)
        {
            var parentResult = await _departmentRepository.GetByIdAsync(parentId.Value, cancellationToken);

            if (parentResult.IsFailure)
            {
                return parentResult.Error.ToErrors();
            }

            parent = parentResult.Value;
        }

        var departmentLocations =
            command.Request.LocationIds.Select(lId => new DepartmentLocation(departmentId, new LocationId(lId)));

        var departmentResult = parent == null
            ? Department.CreateParent(name, identifier, departmentLocations, departmentId)
            : Department.CreateChild(name, identifier, parent, departmentLocations, departmentId);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        var addDepartmentResult = await _departmentRepository.AddAsync(departmentResult.Value, cancellationToken);

        if (addDepartmentResult.IsFailure)
        {
            return addDepartmentResult.Error.ToErrors();
        }

        _logger.LogInformation("Подразделение создано с идентификатором - {departmentId}", departmentResult.Value.Id.Value);

        return departmentResult.Value.Id.Value;
    }
}