using Newtonsoft.Json;

namespace SuperGameSystemBasic
{
    public class Setting
    {
        [JsonProperty]
        public bool ScanLines { get; set; } = true;

        [JsonProperty]
        public bool OverScan { get; set; } = true;

        [JsonProperty]
        public bool ShowHints { get; set; } = true;
    }
}