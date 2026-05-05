using Application.DTOs.Auth;

namespace Application.Interfaces.IServices;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserSummaryDto?> GetUserById(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSummaryDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<List<UserSummaryDto>> SearchUsersByUsernameAsync(string username, int take = 20, CancellationToken cancellationToken = default);
    Task<AuthResponse> GoogleLoginAsync(string code, CancellationToken cancellationToken);

}
