using System.Collections.Generic;
using Newtonsoft.Json;

namespace coinTicker.Models.Feed
{
    public class Channel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("product_ids")]
        public List<string> ProductIds { get; set; }
    }
}
