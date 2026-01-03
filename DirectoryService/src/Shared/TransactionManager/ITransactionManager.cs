using CSharpFunctionalExtensions;
using Shared.Fails;

namespace Shared.TransactionManager;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken);
}