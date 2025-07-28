namespace ProductPriceService.Services
{
    public interface IPriceService
    {

        Task<int> GetPriceByProductIdAsync(int productId);
 
        Task CreatePriceAsync(int productId, int price);


        Task UpdatePriceAsync(int productId, int newPrice);

        Task DeletePriceAsync(int productId);
    }
}
