using ProductCatalogService.DTO;
using ProductCatalogService.Models;

namespace ProductCatalogService.Mapper
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name
            };
        }

        public static Product ToEntity(ProductDto dto)
        {
            return new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name
            };
        }

        public static List<ProductDto> ToDtoList(IEnumerable<Product> products)
        {
            return products.Select(p => ToDto(p)).ToList();
        }

        public static List<Product> ToEntityList(IEnumerable<ProductDto> dtos)
        {
            return dtos.Select(dto => ToEntity(dto)).ToList();
        }

        public static ProductWithPriceDto ToProductWithPriceDto(Product product, int price)
        {
            return new ProductWithPriceDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ProductPrice = price
            };
        }
      

        public static List<ProductWithPriceDto> ToProductWithPriceDtoList(IEnumerable<Product> products, IEnumerable<int> prices)
        {
            var priceList = prices.ToList();
            return products.Select((p, index) => ToProductWithPriceDto(p, priceList[index])).ToList();
        }

      

        

            

    }

}
