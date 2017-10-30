using Newtonsoft.Json;

namespace coinTicker.Models
{
    public class Product
    {
        public string Id { get; set; }
        [JsonProperty("base_currency")]
        public string SourceCurrency { get; set; }
        [JsonProperty("quote_currency")]
        public string TargetCurrency { get; set; }
    }
}