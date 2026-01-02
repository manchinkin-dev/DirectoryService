using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared.Errors;

namespace DirectoryService.Application.Departments;

public interface IDepartmentRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken = default);

    Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<UnitResult<Errors>> CheckExisting(Guid[] departmentIds, CancellationToken cancellationToken = default);
}