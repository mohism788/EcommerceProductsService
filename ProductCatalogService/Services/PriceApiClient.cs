using System.Text.Json;
using System.Text;
using ProductCatalogService.DTO;


namespace ProductCatalogService.Services
{
    public class PriceApiClient : IPriceApiClient
    {
        private readonly IHttpClientFactory _httpClient;
        
        
        public PriceApiClient(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            
        }


        public async Task<bool> CreatePriceAsync(int productId, int price)
        {
            var client = _httpClient.CreateClient();
            var payload = new
            {
                ProductId = productId,
                ProductPrice = price
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"https://localhost:7151/api/Prices", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePriceByProductIdAsync(int productId)
        {
            var client = _httpClient.CreateClient();
            var payload = productId;
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync($"https://localhost:7151/api/Prices/DeletePrice{productId}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to delete price for product ID {productId}. Status code: {response.StatusCode}.");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseString))
            {
                throw new Exception($"Price for product ID {productId} not found or could not be deleted.");
            }
            return true;

        }

        public async Task<ProductIdPriceDto> GetPriceByProductIdAsync(int productId)
        {
            /*var client = _httpClient.CreateClient();
            var response = await client.GetAsync($"https://localhost:7151/api/Prices/GetPriceByProductId/{productId}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get price for product ID {productId}. Status code: {response.StatusCode}");

            var responseString = await response.Content.ReadAsStringAsync();

            var priceDto = JsonSerializer.Deserialize<ProductIdPriceDto>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (priceDto == null)
                throw new Exception($"Price for product ID {productId} not found.");

            return priceDto;*/

            var client = _httpClient.CreateClient();
            var payload = productId;
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"https://localhost:7151/api/Prices/GetPriceByProductId{productId}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get price for product ID {productId}. Status code: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var priceDto = JsonSerializer.Deserialize<ProductIdPriceDto>(responseString
                                                                         ,new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (priceDto == null)
            {
                throw new Exception($"Price for product ID {productId} not found.");
            }
            return priceDto;
        }

        public async Task<IEnumerable<ProductWithPriceDto>> GetPricesByProductIdsAsync(IEnumerable<ProductDto> products)
        {
            var client = _httpClient.CreateClient();
            // Step 1: Build the payload with all product IDs
            var payload = new ProductIdsRequestDto
            {
                ProductIds = products.Select(p => p.ProductId).ToList()
            };

            // Step 2: Serialize to JSON
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            // Step 3: Send single POST request to the Price API
            var response = await client.PostAsync("https://localhost:7151/api/Prices/GetPricesByProductIds", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get prices. Status code: {response.StatusCode}");
            }

            // Step 4: Deserialize the list of prices
            var responseString = await response.Content.ReadAsStringAsync();
            var prices = JsonSerializer.Deserialize<List<ProductIdPriceDto>>(responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
);

            // Step 5: Merge the prices with product names
            var result = products.Select(product =>
            {
                var priceEntry = prices.FirstOrDefault(p => p.ProductId == product.ProductId);
                return new ProductWithPriceDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    ProductPrice = priceEntry?.ProductPrice ?? 0 // default to 0 if price not found
                };
            });

            return result;
        }

        public async Task<ProductIdPriceDto> UpdatePriceByProductIdAsync(int productId, int newPrice)
        {
            var client = _httpClient.CreateClient();
            var payload = new
            {
                ProductId = productId,
                ProductPrice = newPrice
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PutAsync($"https://localhost:7151/api/Prices/UpdateProductPrice{productId}", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update price for product ID {productId}. Status code: {response.StatusCode}");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var priceDto = JsonSerializer.Deserialize<ProductIdPriceDto>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (priceDto == null)
            {
                throw new Exception($"Price for product ID {productId} not found.");
            }
            return await Task.FromResult(priceDto);
        }
    }
}
