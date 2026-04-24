using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.CheckIn;

public class CheckInStartResponseDto
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string InputMode { get; set; } = string.Empty;
    public string FirstQuestion { get; set; } = string.Empty;
}
    