using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using coinTicker.Models;

namespace coinTicker.Services
{
    public interface IDataService
    {
        Task<Ticker> GetTickerByProduct(Product product);
        Task<IEnumerable<Product>> GetAllProducts();
    }
}
