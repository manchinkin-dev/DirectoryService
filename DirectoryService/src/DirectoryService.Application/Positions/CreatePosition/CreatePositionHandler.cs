using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Fails;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionHandler
{
    private readonly IPositionRepository _positionRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IValidator<CreatePositionCommand> _validator;

    public CreatePositionHandler(
        IPositionRepository positionRepository,
        IDepartmentRepository departmentRepository,
        IValidator<CreatePositionCommand> validator,
        ILogger<CreatePositionHandler> logger)
    {
        _positionRepository = positionRepository;
        _validator = validator;
        _logger = logger;
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var positionId = new PositionId(Guid.NewGuid());
        var name = PositionName.Create(command.Request.Name).Value;

        if (await _positionRepository.NameAlreadyExistsAsync(name, cancellationToken))
        {
            return GeneralErrors.AlreadyExist().ToErrors();
        }

        var description = PositionDescription.Create(command.Request.Description).Value;

        var checkExistingResult = await _departmentRepository.CheckExisting(command.Request.DepartmentIds, cancellationToken);

        if (checkExistingResult.IsFailure)
        {
            return checkExistingResult.Error;
        }

        var departmentIds =
            command.Request.DepartmentIds.Select(dp => new DepartmentPosition(new DepartmentId(dp), positionId));

        var position = new Position(positionId, name, description, departmentIds);

        var positionResult = await _positionRepository.AddAsync(position, cancellationToken);
        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        _logger.LogInformation("Должность создана с идентификатором - {positionId}", position.Id.Value);

        return position.Id.Value;
    }
}