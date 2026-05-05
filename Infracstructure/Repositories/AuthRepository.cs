using Application.DTOs.Auth;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuthRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(x => x.Username.ToLower() == normalizedUsername, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string normalizedIdentifier, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Email.ToLower() == normalizedIdentifier || x.Username.ToLower() == normalizedIdentifier,
                cancellationToken);
    }


    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }


    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.GoogleId == googleId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalized, cancellationToken);
    }

    public async Task<List<User>> SearchByUsernameAsync(string username, int take, CancellationToken cancellationToken = default)
    {
        var normalized = username.Trim().ToLowerInvariant();

        return await _dbContext.Users
            .Where(x => x.Username.ToLower().Contains(normalized))
            .OrderBy(x => x.Username.ToLower() == normalized ? 0 : 1)
            .ThenBy(x => x.Username)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
}
