namespace Alpha.VKLib.Models.Audio
{
    using System.Text.Json.Serialization;

    public class VKPlaylist
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("owner_id")]
        public long OwnerId { get; set; }
        [JsonPropertyName("create_time")]
        public long CreateTime { get; set; }
        [JsonPropertyName("upload_time")]
        public long UpdateTime { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("followers")]
        public int Followers { get; set; }
        [JsonPropertyName("plays")]
        public int Plays { get; set; }
        [JsonPropertyName("is_following")]
        public bool IsFollowing { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
        [JsonPropertyName("thumbs")]
        public List<VKThumb> Thumbs { get; set; }
    }
}
