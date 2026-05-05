using Application.DTOs.Achievement;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Mappers
{
    public static class AchievementMapper
    {
        public static AchievementDto ToDto(this Achievement achievement)
        {
            return new AchievementDto
            {
                Id = achievement.Id,
                Code = achievement.Code,
                Description = achievement.Description,
                IconUrl = achievement.IconUrl,
                TargetValue = achievement.TargetValue,
                IsActive = achievement.IsActive,
            };
        }

        public static List<AchievementDto> ToListDto(this List<Achievement> achievements)
        {
            return achievements.Select(ToDto).ToList();
        }

        public static UserAchievementDto ToUserAchievementDto(this UserAchievement userAchievement)
        {
            var targetValue = userAchievement.Achievement?.TargetValue ?? 0;

            return new UserAchievementDto
            {
                Id = userAchievement.Id,
                UserId = userAchievement.UserId,
                AchievementId = userAchievement.AchievementId,
                IsUnlocked = userAchievement.IsUnlocked,
                ProgressValue = userAchievement.ProgressValue,
                ProgressPercentage = targetValue > 0 ? (double)userAchievement.ProgressValue / targetValue * 100 : 0,
                UnlockedAt = userAchievement.UnlockedAt,
                Achievement = userAchievement.Achievement != null ? ToDto(userAchievement.Achievement) : null,
                IsCompleted = userAchievement.ProgressValue >= targetValue && targetValue > 0
            };
        }
        public static List<UserAchievementDto> ToUserAchievementDtoList(this List<UserAchievement> userAchievements)
        {
            return userAchievements.Select(ToUserAchievementDto).ToList();
        }

        public static AchievementProgressUpdateDto ToProgressUpdateDto(this UserAchievement userAchievement, int previousProgress, bool unlockedNow)
        {
            var targetValue = userAchievement.Achievement?.TargetValue ?? 0;
            var code = userAchievement.Achievement?.Code ?? string.Empty;

            return new AchievementProgressUpdateDto
            {
                Code = code,
                PreviousProgress = previousProgress,
                CurrentProgress = userAchievement.ProgressValue,
                TargetValue = targetValue,
                UnlockedNow = unlockedNow,
                UnlockedAt = userAchievement.UnlockedAt
            };
        }
    }
}
