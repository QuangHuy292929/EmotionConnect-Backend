using Application.DTOs.Achievement;
using Application.DTOs.OutboxMessage;
using Application.DTOs.OutboxMessage.Payloads;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;
using System.Text.Json;

namespace Infracstructure.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboxMessageService _outboxMessageService;

        public AchievementService(IUnitOfWork unitOfWork, IOutboxMessageService outboxMessageService)
        {
            _unitOfWork = unitOfWork;
            _outboxMessageService = outboxMessageService;
        }

        public async Task<List<AchievementDto>> GetActiveAchievementsAsync(CancellationToken cancellationToken = default)
        {
            var achievements = await _unitOfWork.AchievementRepository.GetActiveAsync(cancellationToken);
            return achievements.ToListDto();
        }

        public async Task<List<UserAchievementDto>> GetMyAchievementsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            var userAchievements = await _unitOfWork.UserAchievementRepository.GetByUserIdAsync(userId, cancellationToken);
            return userAchievements.ToUserAchievementDtoList();
        }

        public async Task<UserAchievementDto?> GetUserAchievementAsync(
            Guid userId,
            Guid achievementId,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            if (achievementId == Guid.Empty)
            {
                throw new BadRequestException("Achievement ID cannot be empty.");
            }

            var achievement = await _unitOfWork.AchievementRepository.GetByIdAsync(achievementId, cancellationToken);
            if (achievement is null)
            {
                throw new NotFoundException($"Achievement with ID {achievementId} not found.");
            }

            var userAchievement = await _unitOfWork.UserAchievementRepository.GetByUserAndAchievementAsync(
                userId,
                achievementId,
                cancellationToken);

            return userAchievement?.ToUserAchievementDto();
        }

        public async Task<UserAchievementDto?> GetUserAchievementByCodeAsync(
            Guid userId,
            string code,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            var achievement = await ValidateCodeAndReturnAchievementAsync(code, cancellationToken);

            var userAchievement = await _unitOfWork.UserAchievementRepository.GetByUserAndAchievementAsync(
                userId,
                achievement.Id,
                cancellationToken);

            return userAchievement?.ToUserAchievementDto();
        }

        public async Task<UserAchievementDto> InitializeProgressAsync(
            Guid userId,
            Guid achievementId,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            if (achievementId == Guid.Empty)
            {
                throw new BadRequestException("Achievement ID cannot be empty.");
            }

            var achievement = await _unitOfWork.AchievementRepository.GetByIdAsync(achievementId, cancellationToken);
            if (achievement is null)
            {
                throw new NotFoundException($"Achievement with ID {achievementId} not found.");
            }

            var userAchievement = await _unitOfWork.UserAchievementRepository.GetByUserAndAchievementAsync(
                userId,
                achievementId,
                cancellationToken);

            if (userAchievement is not null)
            {
                return userAchievement.ToUserAchievementDto();
            }

            var newUserAchievement = CreateUserAchievement(userId, achievement);

            await _unitOfWork.UserAchievementRepository.AddAsync(newUserAchievement, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return newUserAchievement.ToUserAchievementDto();
        }

        public async Task<AchievementProgressUpdateDto> IncrementProgressAsync(
            Guid userId,
            string achievementCode,
            int amount = 1,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            if (amount <= 0)
            {
                throw new BadRequestException("Increment amount must be greater than zero.");
            }

            var achievement = await ValidateCodeAndReturnAchievementAsync(achievementCode, cancellationToken);
            EnsureAchievementIsActive(achievement);

            var userAchievement = await GetOrCreateUserAchievementAsync(userId, achievement, cancellationToken);
            var previousProgress = userAchievement.ProgressValue;

            if (userAchievement.IsUnlocked)
            {
                return userAchievement.ToProgressUpdateDto(previousProgress, unlockedNow: false);
            }

            userAchievement.ProgressValue = Math.Min(
                userAchievement.ProgressValue + amount,
                achievement.TargetValue);

            var unlockedNow = TryUnlock(userAchievement, achievement);
            Touch(userAchievement);

            await _unitOfWork.SaveChangeAsync(cancellationToken);
            if (unlockedNow)
            {
                await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
                {
                    EventType = EventType.AchievementUnlocked,
                    AggregateType = null,
                    AggregateId = userAchievement.Id,
                    PayloadJson = SerializePayload(new AchievementUnlockedPayload
                    {
                        UserId = userId,
                        AchievementCode = achievement.Code
                    })
                }, cancellationToken);
            }


            return userAchievement.ToProgressUpdateDto(previousProgress, unlockedNow);
        }

        public async Task<AchievementProgressUpdateDto> SetProgressAsync(
            Guid userId,
            string achievementCode,
            int progressValue,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            if (progressValue < 0)
            {
                throw new BadRequestException("Progress value cannot be negative.");
            }

            var achievement = await ValidateCodeAndReturnAchievementAsync(achievementCode, cancellationToken);
            EnsureAchievementIsActive(achievement);

            var userAchievement = await GetOrCreateUserAchievementAsync(userId, achievement, cancellationToken);
            var previousProgress = userAchievement.ProgressValue;

            if (!userAchievement.IsUnlocked)
            {
                userAchievement.ProgressValue = Math.Min(progressValue, achievement.TargetValue);
            }
            else
            {
                userAchievement.ProgressValue = Math.Max(
                    Math.Min(progressValue, achievement.TargetValue),
                    userAchievement.ProgressValue);
            }

            var unlockedNow = TryUnlock(userAchievement, achievement);
            Touch(userAchievement);

            await _unitOfWork.SaveChangeAsync(cancellationToken);
            if (unlockedNow)
            {
                await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
                {
                    EventType = EventType.AchievementUnlocked,
                    AggregateType = null,
                    AggregateId = userAchievement.Id,
                    PayloadJson = SerializePayload(new AchievementUnlockedPayload
                    {
                        UserId = userId,
                        AchievementCode = achievement.Code
                    })
                }, cancellationToken);
            }


            return userAchievement.ToProgressUpdateDto(previousProgress, unlockedNow);
        }

        public async Task<UserAchievementDto> UnlockAsync(
            Guid userId,
            string achievementCode,
            CancellationToken cancellationToken = default)
        {
            await EnsureUserExistsAsync(userId, cancellationToken);

            var achievement = await ValidateCodeAndReturnAchievementAsync(achievementCode, cancellationToken);
            EnsureAchievementIsActive(achievement);

            var userAchievement = await GetOrCreateUserAchievementAsync(userId, achievement, cancellationToken);
            var unlockedNow = false;
            if (!userAchievement.IsUnlocked)
            {
                userAchievement.ProgressValue = Math.Max(userAchievement.ProgressValue, achievement.TargetValue);
                userAchievement.IsUnlocked = true;
                userAchievement.UnlockedAt = DateTime.UtcNow;
                Touch(userAchievement);

                await _unitOfWork.SaveChangeAsync(cancellationToken);
                unlockedNow = true;
            }

            if (unlockedNow)
            {
                await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
                {
                    EventType = EventType.AchievementUnlocked,
                    AggregateType = null,
                    AggregateId = userAchievement.Id,
                    PayloadJson = SerializePayload(new AchievementUnlockedPayload
                    {
                        UserId = userId,
                        AchievementCode = achievement.Code
                    })
                }, cancellationToken);
            }

            return userAchievement.ToUserAchievementDto();
        }

        public async Task<AchievementProgressUpdateDto> ProcessEventAsync(
            Guid userId,
            string achievementCode,
            int amount = 1,
            CancellationToken cancellationToken = default)
        {
            return await IncrementProgressAsync(userId, achievementCode, amount, cancellationToken);
        }

        private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
            {
                throw new BadRequestException("User ID cannot be empty.");
            }

            var exists = await _unitOfWork.AuthRepository.ExistsByIdAsync(userId, cancellationToken);
            if (!exists)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }
        }

        private async Task<Achievement> ValidateCodeAndReturnAchievementAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BadRequestException("Achievement code cannot be null or empty.");
            }

            var achievement = await _unitOfWork.AchievementRepository.GetByCodeAsync(code.Trim(), cancellationToken);
            if (achievement is null)
            {
                throw new NotFoundException($"Achievement with code {code} not found.");
            }

            return achievement;
        }

        private static void EnsureAchievementIsActive(Achievement achievement)
        {
            if (!achievement.IsActive)
            {
                throw new ConflictException("Achievement is not active.");
            }
        }

        private async Task<UserAchievement> GetOrCreateUserAchievementAsync(
            Guid userId,
            Achievement achievement,
            CancellationToken cancellationToken)
        {
            var userAchievement = await _unitOfWork.UserAchievementRepository.GetByUserAndAchievementAsync(
                userId,
                achievement.Id,
                cancellationToken);

            if (userAchievement is not null)
            {
                return userAchievement;
            }

            var newUserAchievement = CreateUserAchievement(userId, achievement);
            await _unitOfWork.UserAchievementRepository.AddAsync(newUserAchievement, cancellationToken);

            return newUserAchievement;
        }

        private static UserAchievement CreateUserAchievement(Guid userId, Achievement achievement)
        {
            return new UserAchievement
            {
                UserId = userId,
                AchievementId = achievement.Id,
                ProgressValue = 0,
                IsUnlocked = false,
                Achievement = achievement
            };
        }

        private static bool TryUnlock(UserAchievement userAchievement, Achievement achievement)
        {
            if (userAchievement.IsUnlocked || userAchievement.ProgressValue < achievement.TargetValue)
            {
                return false;
            }

            userAchievement.IsUnlocked = true;
            userAchievement.UnlockedAt = DateTime.UtcNow;
            return true;
        }

        private static void Touch(UserAchievement userAchievement)
        {
            userAchievement.UpdatedAt = DateTime.UtcNow;
        }
        private static string SerializePayload<T>(T payload)
        {
            return JsonSerializer.Serialize(payload);
        }

    }
}
