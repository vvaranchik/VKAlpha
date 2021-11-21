namespace Alpha.VKLib.Models.Audio
{
    using System.Text.Json.Serialization;

    public class VKThumb
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("photo_34")]
        public string Photo34 { get; set; }
        [JsonPropertyName("photo_68")]
        public string Photo68 { get; set; }
        [JsonPropertyName("photo_135")]
        public string Photo135 { get; set; }
        [JsonPropertyName("photo_270")]
        public string Photo270 { get; set; }
        [JsonPropertyName("photo_300")]
        public string Photo300 { get; set; }
        [JsonPropertyName("photo_600")]
        public string Photo600 { get; set; }
        [JsonPropertyName("photo_1200")]
        public string Photo1200 { get; set; }
    }
}
