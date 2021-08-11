using Newtonsoft.Json;

namespace VKAlpha.Resources.Lang
{
    public class LangModel
    {
        [JsonProperty(Required = Required.Always)]
        public string language { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Friends { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string LogOut { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string MyAudios { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Settings { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Play { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Pause { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Next { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Prev { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string ShuffleOff { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string ShuffleOn { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string RepeatOff { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string RepeatOn { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Mute { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string MaxVol { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string EditSong { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string DelSong { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AddSong { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Lyrics { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string GetFriends { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Login { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Password { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string GoBack { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string UpdateMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AboutMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string HotkeyMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AccountMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string InterfaceMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string aLogIn { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AuthFailed { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AccessDenied { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Language { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string CurrentTheme { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string AppearanceCat { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Playlists { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string My { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string strFormatFriends { get; set; }
    }
}
