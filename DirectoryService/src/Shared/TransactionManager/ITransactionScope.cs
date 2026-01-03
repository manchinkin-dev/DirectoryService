using CSharpFunctionalExtensions;
using Shared.Fails;

namespace Shared.TransactionManager;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}