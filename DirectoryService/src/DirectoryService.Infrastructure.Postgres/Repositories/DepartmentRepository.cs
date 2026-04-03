using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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

    public async Task<Result<Department, Error>> GetByIdWithLocks(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Departments
            .FromSql($"SELECT * FROM departments WHERE id = {departmentId} FOR UPDATE")
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            return GeneralErrors.NotFound(departmentId);

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

    public Task<UnitResult<Error>> LockDescendantsByPath(string oldPath, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Departments
                .FromSqlInterpolated(
                    $"SELECT * FROM departments WHERE path <@ {oldPath}::ltree AND path != {oldPath}::ltree FOR UPDATE");

            return Task.FromResult(UnitResult.Success<Error>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка блокировки потомков");
            return Task.FromResult(UnitResult.Failure(GeneralErrors.Failure("Ошибка блокировки потомков")));
        }
    }

    public Task<UnitResult<Error>> BulkUpdateDescendantsPathAndDepth(
        string oldPathValue,
        string pathValue,
        int depth,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Departments
                .FromSqlInterpolated(
                    $"""
                     UPDATE departments
                     SET path={pathValue}::ltree || subpath(path, nlevel({oldPathValue}::ltree)),
                         depth= departments.depth + {depth},
                         updated_at={DateTime.UtcNow}
                     WHERE path <@{oldPathValue}::ltree and path!={oldPathValue}::ltree
                     """);

            return Task.FromResult(UnitResult.Success<Error>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обновления потомков");
            return Task.FromResult(UnitResult.Failure(GeneralErrors.Failure("Ошибка обновления потомков")));
        }
    }

    public Task<UnitResult<Error>> BulkUpdateDescendantsPath(
        string oldPathValue,
        string pathValue,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Departments
                .FromSqlInterpolated(
                    $"""
                     UPDATE departments
                     SET path={pathValue}::ltree || subpath(path, nlevel({oldPathValue}::ltree)),
                         updated_at={DateTime.UtcNow}
                     WHERE path <@{oldPathValue}::ltree and path!={oldPathValue}::ltree
                     """);

            return Task.FromResult(UnitResult.Success<Error>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обновления потомков");
            return Task.FromResult(UnitResult.Failure(GeneralErrors.Failure("Ошибка обновления потомков")));
        }
    }

    public async Task<UnitResult<Error>> DeactivateLocationsOrPositions(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = """
                               WITH deactivated_locations AS (
                                   UPDATE locations l
                                       SET is_active = false,
                                           deleted_at = now(),
                                           updated_at = now()
                                       FROM department_locations dl
                                       WHERE l.id = dl.location_id
                                           AND dl.department_id = @departmentId
                                           AND l.is_active
                                           AND NOT EXISTS (
                                               SELECT 1
                                               FROM department_locations dl2
                                               JOIN departments d ON d.id = dl2.department_id AND d.is_active
                                               WHERE dl2.location_id = dl.location_id
                                                   AND d.id != @departmentId
                                           )
                               )
                               UPDATE positions p
                               SET is_active = false,
                                   deleted_at = now(),
                                   updated_at = now()
                               FROM department_positions dp
                               WHERE p.id = dp.position_id
                                   AND dp.department_id = @departmentId
                                   AND p.is_active
                                   AND NOT EXISTS (
                                       SELECT 1
                                       FROM department_positions dp2
                                       JOIN departments d ON d.id = dp2.department_id AND d.is_active
                                           WHERE dp2.position_id = dp.position_id
                                           AND d.id != @departmentId
                                   )
                               """;

            await _dbContext.Database.ExecuteSqlRawAsync(
                sql,
                parameters: [new NpgsqlParameter("departmentId", departmentId.Value)],
                cancellationToken);

            return await Task.FromResult(UnitResult.Success<Error>());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Ошибка soft удаления локаций или позиций у подразделения с идентификатором - {departmentId}",
                departmentId.Value);

            return await Task.FromResult(UnitResult.Failure(GeneralErrors.Failure(
                $"Ошибка soft удаления локаций или позиций у подразделения с идентификатором - {departmentId.Value}")));
        }
    }
}