
using Microsoft.EntityFrameworkCore;
using ProductPriceService.DataAccess;
using ProductPriceService.Models;

namespace ProductPriceService.Services
{
    public class PriceService : IPriceService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<PriceService> _logger;

        public PriceService(ApplicationDbContext applicationDbContext, ILogger<PriceService> logger )
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }
        public async Task CreatePriceAsync(int productId, int price)
        {
            //Log create product for {id}
            _logger.LogInformation($"Creating price for product with ID {productId} and price {price}");
            //check if product exists in the Price DB
            var productExists = await _applicationDbContext.Prices.AnyAsync(p => p.ProductId == productId);
            if (productExists)
            {
                // Log product already exists
                _logger.LogWarning($"Price for product with ID {productId} already exists.");
                throw new InvalidOperationException($"Price for product with ID {productId} already exists.");
            }
            var newPrice = new Prices
            {
                ProductId = productId,
                ProductPrice = price
            };

            var value = await _applicationDbContext.Prices.AddAsync(newPrice);
            await _applicationDbContext.SaveChangesAsync();

            // Log created product with {id}
            _logger.LogInformation($"Created price for product with ID {value.Entity.ProductId} and price {value.Entity.ProductPrice}");



        }

        public async Task<int> GetPriceByProductIdAsync(int productId)
        {
            //if productId is less than 1 OR not found in Prices DB, throw an exception
            if (productId < 1)
            {
                throw new ArgumentException("Product ID must be greater than 0.");
            }
            // Check if product exists in the Prices DB
            var productExists = await _applicationDbContext.Prices.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found in Prices DB.");
            }
            // Log getting price for product with {id}
            _logger.LogInformation($"Retrieving price for product with ID {productId}");
            var price = await _applicationDbContext.Prices
                .Where(p => p.ProductId == productId)
                .Select(p => p.ProductPrice)
                .FirstOrDefaultAsync();

            //  Log retrieved price for product with {id}
            _logger.LogInformation($"Retrieved price for product with ID {productId}: {price}");

            if (price == null)
            {
                throw new KeyNotFoundException($"Price for product with ID {productId} not found.");
            }
                
            
            return price;
        }
    }
}
