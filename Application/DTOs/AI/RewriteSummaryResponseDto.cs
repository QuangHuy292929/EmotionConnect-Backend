using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.AI
{
    public class RewriteSummaryResponseDto
    {
        public string OriginalText { get; set; } = string.Empty;
        public string RewrittenText { get; set; } = string.Empty;
    }
}
