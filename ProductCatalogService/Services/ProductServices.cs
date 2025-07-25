using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogService.DataAccess;
using ProductCatalogService.DTO;
using ProductCatalogService.Models;

namespace ProductCatalogService.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPriceApiClient _priceApiClient;

        public ProductServices(ApplicationDbContext applicationDbContext, IPriceApiClient priceApiClient)
        {
            _applicationDbContext = applicationDbContext;
            _priceApiClient = priceApiClient;
        }

        public IPriceApiClient PriceApiClient { get; }

        public async Task<Product?> CreateProductAsync(Product product, int price)
        {


            var newProduct = await _applicationDbContext.Products.AddAsync(product);


            await _applicationDbContext.SaveChangesAsync();
            var priceCreated = await _priceApiClient.CreatePriceAsync(newProduct.Entity.ProductId, price);
            if (!priceCreated)
            {
                throw new Exception("Failed to create price in PriceAPI");
            }
            // Log product creation

            return product;

        }

        public async Task<Product?> DeleteProductAsync(int id)
        {
            var exist = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (exist == null)
            {
                return null;
            }
            _applicationDbContext.Remove(exist);
            await _applicationDbContext.SaveChangesAsync();
            return exist;
        }

        public async Task<IEnumerable<ProductWithPriceDto>> GetAllProductsAsync()
        {
            // Step 1: Get products from DB
            var products = await _applicationDbContext.Products.ToListAsync();

            // Step 2: Project to DTOs with Id & Name
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name
            }).ToList();

            // Step 3: Call Price API to get prices (returns list of ProductWithPriceDto)
            var productsWithPrices = await _priceApiClient.GetPricesByProductIdsAsync(productDtos);

            return productsWithPrices;
        }


        public async Task<ProductWithPriceDto> GetProductByIdAsync(int productId)
        {
            // Get product from DB (has Id & Name)
            var product = await _applicationDbContext.Products
                           .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                throw new Exception($"Product ID {productId} not found");

            // Call price API to get price
            var priceDto = await _priceApiClient.GetPriceByProductIdAsync(productId);

            // Merge and return
            return new ProductWithPriceDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ProductPrice = priceDto.ProductPrice
            };
        }

    }
}
