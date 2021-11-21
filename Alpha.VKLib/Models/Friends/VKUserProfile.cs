namespace Alpha.VKLib.Models.Friends
{
    using System.Text.Json.Serialization;

    public class VKCountry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    public class VKLastSeen
    {
        [JsonPropertyName("platform")]
        public int PlatformType { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }
    }

    public class VKUserProfile : VKBaseProfile
    {
        [JsonPropertyName("sex")]
        public int Sex { get; set; }
        [JsonPropertyName("online")]
        public int Online { get; set; }
        [JsonPropertyName("has_mobile")]
        public int HasMobile { get; set; }
        [JsonPropertyName("can_post")]
        public int CanPost { get; set; }
        [JsonPropertyName("can_see_all_posts")]
        public int CanSeeAllPosts { get; set; }
        [JsonPropertyName("can_write_private_message")]
        public int CanWritePrivateMessage { get; set; }
        [JsonPropertyName("can_invite_to_chats")]
        public bool CanInviteToChats { get; set; }
        [JsonPropertyName("photo_50")]
        public string Photo50 { get; set; }
        [JsonPropertyName("photo_100")]
        public string Photo100 { get; set; }
        [JsonPropertyName("photo_200_orig")]
        public string Photo200_Orig { get; set; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        [JsonPropertyName("bdate")]
        public string BirthDate { get; set; }
        [JsonPropertyName("mobile_phone")]
        public string MobilePhone { get; set; }
        [JsonPropertyName("home_phone")]
        public string HomePhone { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("track_code")]
        public string TrackCode { get; set; }
        [JsonPropertyName("country")]
        public VKCountry Country { get; set; }
        [JsonPropertyName("last_seen")]
        public VKLastSeen LastSeen { get; set; }
    }
}
