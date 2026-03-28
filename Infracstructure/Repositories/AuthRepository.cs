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

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AnyAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AnyAsync(x => x.Username.ToLower() == normalizedUsername, cancellationToken);
    }

    public Task<User?> GetByEmailOrUsernameAsync(string normalizedIdentifier, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Email.ToLower() == normalizedIdentifier || x.Username.ToLower() == normalizedIdentifier,
                cancellationToken);
    }

    public Task<UserSummaryDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => new UserSummaryDto
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                DisplayName = x.DisplayName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }
}
