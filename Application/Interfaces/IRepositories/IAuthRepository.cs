using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.IRepositories;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailOrUsernameAsync(string normalizedIdentifier, CancellationToken cancellationToken = default);
    Task<UserSummaryDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);



    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
