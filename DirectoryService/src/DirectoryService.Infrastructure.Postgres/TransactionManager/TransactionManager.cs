using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Fails;
using Shared.TransactionManager;

namespace DirectoryService.Infrastructure.TransactionManager;

public class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(
        DirectoryServiceDbContext dbContext,
        ILoggerFactory loggerFactory,
        ILogger<TransactionManager> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var transactionCreateLogger = _loggerFactory.CreateLogger<TransactionScope>();

            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), transactionCreateLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания транзакции");
            return GeneralErrors.Failure("Ошибка создания транзакции");
        }
    }

    public async Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Errors>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка сохранения данных");
            return UnitResult.Failure<Errors>(GeneralErrors.Failure("Ошибка сохранения данных"));
        }
    }
}