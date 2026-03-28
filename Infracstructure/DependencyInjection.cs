using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Infracstructure.Persistence;
using Infracstructure.Repositories;
using Infracstructure.Security;
using Infracstructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infracstructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.UseVector();
            });
        });

        services.AddScoped<PasswordHasher<Domain.Entities.User>>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ICommunityRepository, CommunityRepository>();
        services.AddScoped<IMatchingRepository, MatchingRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IReflectionRepository, ReflectionRepository>();
        services.AddScoped<IEmotionRepository, EmotionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICommunityService, CommunityService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IReflectionService, ReflectionService>();
        services.AddScoped<IEmotionService, EmotionService>();
        services.AddScoped<IAiService, AiService>();

        return services;
    }
}
