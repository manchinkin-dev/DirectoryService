using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared.Fails;

namespace DirectoryService.Application.Departments;

public interface IDepartmentRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken = default);

    Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<Result<Department, Error>> GetByIdWithLocks(Guid departmentId, CancellationToken cancellationToken = default);

    Task<UnitResult<Errors>> CheckExisting(Guid[] departmentIds, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> DeleteLocationsAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> LockDescendantsByPath(string oldPath, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkUpdateDescendantsPathAndDepth(string oldPathValue, string pathValue, int depth, CancellationToken cancellationToken);
}