using System.Text.Json.Serialization;

namespace DShop.AdminPlugin.Responses
{
    public class UserResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
        [JsonPropertyName("sex")]
        public string Sex { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
