using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Fails;

namespace DirectoryService.Infrastructure.Repositories;

public class PositionsRepository : IPositionRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<PositionsRepository> _logger;

    public PositionsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<PositionsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка добавления должности");

            return Error.Failure("position.insert", "Ошибка добавления должности");
        }
    }

    public async Task<bool> NameAlreadyExistsAsync(PositionName name, CancellationToken cancellationToken = default)
    {
        var result =
            await _dbContext.Positions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name && p.IsActive, cancellationToken);

        return result != null;
    }
}