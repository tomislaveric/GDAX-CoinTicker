using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using coinTicker.Models;
using Newtonsoft.Json;

namespace coinTicker.Services
{
    public class DataService : IDataService
    {
        private const string baseUrl = "https://api.gdax.com";

        private async Task<string> GET(string url)
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "coinTicker");
            return await client.DownloadStringTaskAsync(url);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var result = await GET(baseUrl + "/products");
            return JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
        }

        public async Task<Ticker> GetTickerByProduct(Product product)
        {
            var result = await GET(baseUrl + "/products/" + product.Id + "/ticker");
            return JsonConvert.DeserializeObject<Ticker>(result);
        }
    }
}
