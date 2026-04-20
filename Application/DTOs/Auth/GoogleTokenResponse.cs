using System.Text.Json.Serialization;

namespace Application.DTOs.Auth
{
    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
