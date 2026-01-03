using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using Shared.Errors;

namespace DirectoryService.Application.Positions;

public interface IPositionRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken = default);

    Task<bool> NameAlreadyExistsAsync(PositionName name, CancellationToken cancellationToken = default);
}