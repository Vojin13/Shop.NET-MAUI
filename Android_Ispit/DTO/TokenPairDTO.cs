using Newtonsoft.Json;

namespace Android_Ispit.DTO
{
    public class TokenPairDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
