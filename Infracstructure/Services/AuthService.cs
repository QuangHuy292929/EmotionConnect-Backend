using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Infracstructure.Mappers;
using Infracstructure.Security;
using Infracstructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Infracstructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public AuthService(
        IUnitOfWork unitOfWork,
        PasswordHasher<User> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IHttpClientFactory httpClientFactory,
        IConfiguration config

        )
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        if (await _unitOfWork.AuthRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("Email is already in use.");
        }

        if (await _unitOfWork.AuthRepository.UsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            throw new ConflictException("Username is already in use.");
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = normalizedEmail,
            DisplayName = request.DisplayName.Trim()
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _unitOfWork.AuthRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var identifier = request.EmailOrUsername.Trim().ToLowerInvariant();

        var user = await _unitOfWork.AuthRepository.GetByEmailOrUsernameAsync(identifier, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Invalid credentials.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizeException("Invalid credentials.");
        }

        return BuildAuthResponse(user);
    }

    public async Task<UserSummaryDto?> GetUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.AuthRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        return user.ToSummaryDto();
    }

    public async Task<UserSummaryDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException("User ID cannot be empty.");
        }

        ArgumentNullException.ThrowIfNull(request);

        var user = await _unitOfWork.AuthRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            user.DisplayName = request.DisplayName.Trim();
        }

        if (request.Bio is not null)
        {
            user.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();
        }

        if (request.AvatarUrl is not null)
        {
            user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.AuthRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return user.ToSummaryDto();
    }

    public async Task<List<UserSummaryDto>> SearchUsersByUsernameAsync(
        string username,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new BadRequestException("Username search term cannot be empty.");
        }

        if (take <= 0)
        {
            throw new BadRequestException("Take must be greater than zero.");
        }

        var users = await _unitOfWork.AuthRepository.SearchByUsernameAsync(username, take, cancellationToken);
        return users.Select(x => x.ToSummaryDto()).ToList();
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAtUtc) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new UserSummaryDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName
            }
        };
    }

    public async Task<AuthResponse> GoogleLoginAsync(string code, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();

        // ── Bước 1: Đổi code → access_token ──
        var tokenRes = await httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _config["Google:ClientId"]!,
                ["client_secret"] = _config["Google:ClientSecret"]!,
                ["redirect_uri"] = _config["Google:RedirectUri"]!,
                ["grant_type"] = "authorization_code"
            }), cancellationToken);

        if (!tokenRes.IsSuccessStatusCode)
            throw new UnauthorizeException("Không thể xác thực với Google.");

        var tokenData = await tokenRes.Content
            .ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: cancellationToken)
            ?? throw new UnauthorizeException("Google trả về token không hợp lệ.");

        // ── Bước 2: Lấy thông tin user từ Google ──
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userInfo = await httpClient.GetFromJsonAsync<GoogleUserInfo>(
            "https://www.googleapis.com/oauth2/v3/userinfo", cancellationToken)
            ?? throw new UnauthorizeException("Không lấy được thông tin user từ Google.");

        // ── Bước 3: Tìm user trong DB ──
        var user = await _unitOfWork.AuthRepository.GetByGoogleIdAsync(userInfo.Sub, cancellationToken)
                ?? await _unitOfWork.AuthRepository.GetByEmailAsync(userInfo.Email, cancellationToken);

        if (user is null)
        {
            // ── Bước 4a: Tạo user mới ──
            user = new User
            {
                Username = await GenerateUniqueUsernameAsync(userInfo.Email, cancellationToken),
                Email = userInfo.Email.Trim().ToLowerInvariant(),
                DisplayName = userInfo.Name,
                GoogleId = userInfo.Sub,
                IsGoogleAccount = true,
                PasswordHash = string.Empty
            };
            await _unitOfWork.AuthRepository.AddAsync(user, cancellationToken);
        }
        else if (string.IsNullOrWhiteSpace(user.GoogleId))
        {
            // ── Bước 4b: Link Google vào account email cũ ──
            user.GoogleId = userInfo.Sub;
            user.IsGoogleAccount = true;
            await _unitOfWork.AuthRepository.UpdateAsync(user, cancellationToken);
        }

        // ── Bước 5: Commit DB ──
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        // ── Bước 6: Tạo JWT và trả về ──
        return BuildAuthResponse(user);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email, CancellationToken ct)
    {
        var baseName = email.Split('@')[0].ToLowerInvariant();
        var username = baseName;
        var suffix = 1;

        while (await _unitOfWork.AuthRepository.UsernameExistsAsync(username, ct))
            username = $"{baseName}{suffix++}";

        return username;
    }
}

