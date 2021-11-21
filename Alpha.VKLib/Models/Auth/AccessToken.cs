namespace Alpha.VKLib.Models.Auth
{
    using System.Text.Json.Serialization;

    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }
        [JsonPropertyName("expire_time")]
        public long ExpiresIn { get; set; }
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
    }
}
