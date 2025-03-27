using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface IProductService
    {
        Product GetProductById(int id);
        IEnumerable<Product> GetAllProducts();
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
    }
}
