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
            var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

            try
            {
               
                var newProduct = await _applicationDbContext.Products.AddAsync(product);
                await _applicationDbContext.SaveChangesAsync();
                var priceCreated = await _priceApiClient.CreatePriceAsync(newProduct.Entity.ProductId, price);
                if (!priceCreated)
                {
                    throw new Exception("Failed to create price in PriceAPI");
                }
                // Log product creation
                await transaction.CommitAsync();

                return product;
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw; // rethrow so controller returns proper error
            }

        }

        public async Task<Product?> DeleteProductAsync(int id)
        {
            var exist = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (exist == null)
            {
                return null;
            }
            // Call Price API to delete price
            var priceDeleted = await _priceApiClient.DeletePriceByProductIdAsync(id);
            if (!priceDeleted)
            {
                throw new Exception("Failed to delete price in PriceAPI");
            }
            // Remove product from DB
            // Log deleting product with {id}
            _applicationDbContext.Entry(exist).State = EntityState.Deleted;
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
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            

            // Call price API to get price
            var priceDto = await _priceApiClient.GetPriceByProductIdAsync(productId);

            // Merge and return
            return new ProductWithPriceDto
            {
                ProductId = priceDto.ProductId,
                Name = product.Name,
                ProductPrice = priceDto.ProductPrice
            };
        }

        public async Task<ProductDto?> UpdateProductNameAsync(int productId, UpdateProductNameDto productName)
        {
            var exist = await _applicationDbContext.Products.FirstOrDefaultAsync(n => n.ProductId == productId);
            if (exist == null)
            {
                //throw exception with message 
                throw new Exception("This product is not in our Database");
            }

            exist.Name = productName.Name;
            await _applicationDbContext.SaveChangesAsync();

            var updatedProductName = new ProductDto()
            {
                ProductId = productId,
                Name = productName.Name
            };

            return updatedProductName;
        }

        public async Task<ProductWithPriceDto?> UpdateProductPriceAsync(int productId, int newPrice)
        {
            // Get product from DB
            var product = await _applicationDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return await Task.FromResult<ProductWithPriceDto?>(null);
            }
            // Update price in Price API
            var updatedPrice = await _priceApiClient.UpdatePriceByProductIdAsync(productId, newPrice);
            // Merge and return

            return new ProductWithPriceDto
            {
                ProductId = updatedPrice.ProductId,
                Name = product.Name,
                ProductPrice = updatedPrice.ProductPrice
            };

        }
    }
}
