using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.AI
{
    public class ClarifySummaryResponseDto
    {
        public string EmtionAnswer { get; set; } = string.Empty;
        public string IssueAnswer { get; set; } = string.Empty;
        public string DeepDiveAnswer { get; set; } = string.Empty;
        public string ClarifiedSummary { get; set; } = string.Empty;
    }
}
