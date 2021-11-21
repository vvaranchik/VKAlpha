namespace Alpha.VKLib.Models.Friends
{
    using System.Text.Json.Serialization;

    public class VKBaseProfile
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";
    }
}
