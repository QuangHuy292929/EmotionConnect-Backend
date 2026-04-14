using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class GoogleTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
