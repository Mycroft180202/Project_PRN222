using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;

namespace Project_PRN222.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectPrn222Context _context;
        public ProductRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        void IProductRepository.Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        void IProductRepository.Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        IEnumerable<Product> IProductRepository.GetAll()
        {
            return _context.Products.ToList();
        }

        Product IProductRepository.GetById(int id)
        {
            return _context.Products.Find(id);
        }

        void IProductRepository.Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
    }
}
