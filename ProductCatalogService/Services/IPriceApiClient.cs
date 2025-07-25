using ProductCatalogService.DTO;

namespace ProductCatalogService.Services
{
    public interface IPriceApiClient
    {
        Task<bool> CreatePriceAsync(int productId, int price);
        //get price by using a list of productId 
        Task<IEnumerable<ProductWithPriceDto>> GetPricesByProductIdsAsync(IEnumerable<ProductDto> products);

        // Get price by productId
        Task<ProductIdPriceDto> GetPriceByProductIdAsync(int productId);


        //Task<int> GetPriceByProductIdAsync(int productId);
    }
}
