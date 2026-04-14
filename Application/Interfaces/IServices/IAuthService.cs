using Application.DTOs.Auth;

namespace Application.Interfaces.IServices;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserSummaryDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AuthResponse> GoogleLoginAsync(string code, CancellationToken cancellationToken);

}
