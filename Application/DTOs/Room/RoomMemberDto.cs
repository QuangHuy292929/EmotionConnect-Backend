using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Application.DTOs.Room
{
    public class RoomMemberDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
