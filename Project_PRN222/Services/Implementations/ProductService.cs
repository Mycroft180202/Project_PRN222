using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorService _vendorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductService(IProductRepository productRepository, IVendorService vendorService, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _vendorService = vendorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Product GetProductById(int id)
        {
            return _productRepository.GetById(id);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _productRepository.GetAll();
        }

        public void CreateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (string.IsNullOrEmpty(product.ProductName))
            {
                throw new ArgumentException("ProductName is required.");
            }

            var roleId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("RoleId") ?? "0");
            if (roleId == 2) // Vendor
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("UserId"));
                var vendor = _vendorService.GetByUserId(userId).Result; // Lấy Vendor từ UserId
                if (vendor == null)
                {
                    throw new InvalidOperationException("Vendor not found for this user.");
                }
                product.VendorId = vendor.VendorId;
            }

            _productRepository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }
    }
}
