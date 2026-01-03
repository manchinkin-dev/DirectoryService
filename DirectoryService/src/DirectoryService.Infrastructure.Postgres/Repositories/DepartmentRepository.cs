using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Fails;

namespace DirectoryService.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<DepartmentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(
        Department department,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка добавления подразделения");

            return Error.Failure("department.insert", "Ошибка добавления подразделения");
        }
    }

    public async Task<Result<Department, Error>> GetByIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var id = new DepartmentId(departmentId);
        var result = await _dbContext.Departments
            .FindAsync([id], cancellationToken);

        if (result == null)
        {
            return GeneralErrors.NotFound(departmentId, "подразделение");
        }

        return result;
    }

    public async Task<UnitResult<Errors>> CheckExisting(
        Guid[] departmentIds,
        CancellationToken cancellationToken = default)
    {
        var ids = departmentIds.Select(id => new DepartmentId(id));

        var resultIds = await _dbContext.Departments
            .AsNoTracking()
            .Where(d => ids.Contains(d.Id) && d.IsActive)
            .Select(d => d.Id.Value)
            .ToListAsync(cancellationToken);

        var missingIds = departmentIds.Except(resultIds);

        var errors = missingIds
            .Select(m => GeneralErrors.NotFound(m, "идентификатор подразделения"))
            .ToList();

        return errors.Count != 0
            ? UnitResult.Failure(new Errors(errors))
            : UnitResult.Success<Errors>();
    }

    public async Task<UnitResult<Error>> DeleteLocationsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var id = new DepartmentId(departmentId);

            await _dbContext.DepartmentLocations
                .Where(dl => dl.DepartmentId == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка удаления локаций у подразделения - {departmentId}", departmentId);
            return UnitResult.Failure(GeneralErrors.Failure("Ошибка удаления локаций"));
        }

        return UnitResult.Success<Error>();
    }
}