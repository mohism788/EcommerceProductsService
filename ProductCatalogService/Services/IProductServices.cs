using ProductCatalogService.DTO;
using ProductCatalogService.Models;

namespace ProductCatalogService.Services
{
    public interface IProductServices
    {
        //Create new product

        Task<Product> CreateProductAsync(Product product, int price);
        //Get all products
        Task<IEnumerable<ProductWithPriceDto>> GetAllProductsAsync();
        //Get product by id
        Task<ProductWithPriceDto> GetProductByIdAsync(int productId);
        Task<Product?> DeleteProductAsync(int id);
    }
}
