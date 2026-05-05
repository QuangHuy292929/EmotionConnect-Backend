using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Achievement
{
    public class AchievementDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int TargetValue { get; set; }
        public bool IsActive { get; set; }
    }
}
