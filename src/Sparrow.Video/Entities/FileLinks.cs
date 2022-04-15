using Newtonsoft.Json;

namespace Sparrow.Video.Entities
{
    public class FileLinks
    {
        public string Original { get; set; }
        public string Converted { get; set; }
        public string Ts { get; set; }
        [JsonProperty]
        public string Current { get; internal set; }
    }
}
