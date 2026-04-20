using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.CheckIn
{
    public class ConfirmCheckInRequest
    {
        public bool IsConfirmed { get; set; }
        public string? EditedSummary { get; set; }
    }
}
