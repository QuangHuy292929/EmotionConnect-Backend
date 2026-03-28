using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infracstructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IAuthRepository authRepository,
        IRoomRepository roomRepository,
        ICommunityRepository communityRepository,
        IMatchingRepository matchingRepository,
        IMessageRepository messageRepository,
        IReflectionRepository reflectionRepository,
        IEmotionRepository emotionRepository)
    {
        _dbContext = dbContext;
        AuthRepository = authRepository;
        RoomRepository = roomRepository;
        CommunityRepository = communityRepository;
        MatchingRepository = matchingRepository;
        MessageRepository = messageRepository;
        ReflectionRepository = reflectionRepository;
        EmotionRepository = emotionRepository;
    }

    public IAuthRepository AuthRepository { get; }
    public IRoomRepository RoomRepository { get; }
    public ICommunityRepository CommunityRepository { get; }
    public IMatchingRepository MatchingRepository { get; }
    public IMessageRepository MessageRepository { get; }
    public IReflectionRepository ReflectionRepository { get; }
    public IEmotionRepository EmotionRepository { get; }

    public Task<int> SaveChangeAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            return;
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _transaction?.Dispose();
        _dbContext.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
