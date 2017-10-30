using System;
using System.IO;
using System.Reflection;
using coinTicker.Models;
using Newtonsoft.Json;

namespace coinTicker.Services
{
    public class FileService : IFileService
    {
        private const string appName = "coinTicker";
        private readonly string configDirectory;

        public FileService()
        {
            
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            configDirectory = $"{appDataDir}/{appName}/";
            
            if (Directory.Exists(configDirectory)) return;
            
            Directory.CreateDirectory($"{appDataDir}/{appName}/");
            File.WriteAllText($"{configDirectory}/defaultProduct.json", "");
        }

        public void SaveDefaultProduct(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            File.WriteAllText($"{configDirectory}/defaultProduct.json", json);
        }

        public Product GetDefaultProduct()
        {
            var file = File.ReadAllText($"{configDirectory}/defaultProduct.json");
            return string.IsNullOrEmpty(file)
                ? new Product() {Id = "BTC-EUR", TargetCurrency = "EUR"}
                : JsonConvert.DeserializeObject<Product>(file);
        }
    }
}