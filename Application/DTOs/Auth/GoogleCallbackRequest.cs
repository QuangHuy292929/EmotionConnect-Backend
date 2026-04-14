using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class GoogleCallbackRequest
    {
        public string Code { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
