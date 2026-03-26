using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository AuthRepository { get; }

        Task<int> SaveChangeAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
