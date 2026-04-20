using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.CheckIn;

public class CheckInStepResponseDto
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string? NextQuestion { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsAwaitingConfirmation { get; set; }
    public string? Summary { get; set; }
}
