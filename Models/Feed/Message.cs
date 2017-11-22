using System.Collections.Generic;
using Newtonsoft.Json;

namespace coinTicker.Models.Feed
{
    public class Message
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("product_ids")]
        public List<string> ProductIds { get; set; }
        [JsonProperty("channels")]
        public List<object> Channels { get; set; }
    }
}
