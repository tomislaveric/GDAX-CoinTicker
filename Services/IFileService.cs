using coinTicker.Models;

namespace coinTicker.Services
{
    public interface IFileService
    {
        void SaveDefaultProduct(Product product);
        Product GetDefaultProduct();
    }
}