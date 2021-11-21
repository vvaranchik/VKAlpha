namespace Alpha.VKLib.Models.Audio
{
    using System.Text.Json.Serialization;

    public class VKAlbum
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("owner_id")]
        public long OwnerId { get; set; }
        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("thumb")]
        public VKThumb Thumb { get; set; }
    }

    public class VKAudio
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("owner_id")]
        public long OwnerId { get; set; }
        [JsonPropertyName("duration")]
        public long Duration { get; set; }
        [JsonPropertyName("date")]
        public long Date { get; set; }
        [JsonPropertyName("genre_id")]
        public int GenreId { get; set; }
        [JsonPropertyName("is_licensed")]
        public bool IsLicensed { get; set; }
        [JsonPropertyName("short_videos_allowed")]
        public bool ShortVideosAllowed { get; set; }
        [JsonPropertyName("stories_allowed")]
        public bool StoriesAllowed { get; set; }
        [JsonPropertyName("stories_cover_allowed")]
        public bool StoriesCoverAllowed { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("artist")]
        public string Artist { get; set; }
        [JsonPropertyName("access_key")]
        public string AccessKey { get; set; }
        [JsonPropertyName("album")]
        public VKAlbum Album { get; set; }
    }
}
