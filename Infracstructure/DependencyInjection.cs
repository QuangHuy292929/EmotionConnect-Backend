using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Infracstructure.AI;
using Infracstructure.Presence;
using Infracstructure.Persistence;
using Infracstructure.Repositories;
using Infracstructure.Security;
using Infracstructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infracstructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<RedisPresenceOptions>(configuration.GetSection(RedisPresenceOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.UseVector();
            });
        });

        services.Configure<AIServiceOptions>(configuration.GetSection(AIServiceOptions.SectionName));

        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<RedisPresenceOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException("RedisPresence:ConnectionString is not configured.");
            }

            return ConnectionMultiplexer.Connect(options.ConnectionString);
        });

        services.AddHttpClient<IAiService, AiService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AIServiceOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new InvalidOperationException("AIService:BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient();

        services.AddScoped<PasswordHasher<Domain.Entities.User>>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IMatchingRepository, MatchingRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReflectionRepository, ReflectionRepository>();
        services.AddScoped<IEmotionRepository, EmotionRepository>();
        services.AddScoped<ICheckInSessionRepository, CheckInSessionRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IUploadRepository, UploadRepository>();


        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IReflectionService, ReflectionService>();
        services.AddScoped<IEmotionService, EmotionService>();
        services.AddScoped<IUserPresenceService, UserPresenceService>();
        services.AddScoped<ICheckInSessionService, CheckInSessionService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IOutboxMessageService, OutboxMessageService>();
        services.AddScoped<IUploadService, UploadService>();


        services.AddScoped<IOutBoxProcessor, OutboxProcessor>();
        services.AddHostedService<OutboxProcessorBackgroundService>();

        return services;
    }
}
