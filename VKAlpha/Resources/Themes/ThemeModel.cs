using Newtonsoft.Json;
using System.Windows.Media;

namespace VKAlpha.Resources.Themes
{
    public class ThemeModel
    {
        // NOT Nessesary stuff
        [JsonProperty(Required = Required.AllowNull)]
        public string author { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string desc { get; set; }
        // Nessesary stuff
        [JsonProperty(Required = Required.Always)]
        public Brush btnHovered { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush checkBoxChecked { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush checkBoxUncheked { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string name { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string shortname { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush MainBackground { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush BelowMain { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush AboveMain { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush ForegroundMenu { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush ForegroundElements { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush PlayerForegroundBottom { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush TitleListForeground { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush OtherListForeground { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush HintForeground { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush TextBoxBackground { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Brush TextBoxForeground { get; set; }
        [JsonProperty(Required =Required.Always)]
        public Brush listboxItemHovered { get; set; }
    }
}
