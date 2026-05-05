using Application.DTOs.Auth;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Mappers
{
    public static class UserMapper
    {
        public static UserSummaryDto ToSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio
            };
        }
    }
}
