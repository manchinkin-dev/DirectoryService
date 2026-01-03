using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Shared.Fails;
using Shared.TransactionManager;

namespace DirectoryService.Infrastructure.TransactionManager;

public class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(
        IDbTransaction transaction,
        ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _transaction.Commit();

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Error.Failure("transaction.commit", "Ошибка транзакции");
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _transaction.Rollback();

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Error.Failure("transaction.rollback", "Ошибка транзакции");
        }
    }

    public void Dispose() => _transaction.Dispose();
}