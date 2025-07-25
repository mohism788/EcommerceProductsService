using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductPriceService.DTO;
using ProductPriceService.Services;

namespace ProductPriceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase


    {
        private readonly IPriceService _priceService;

        public PricesController(IPriceService priceService)
        {
            _priceService = priceService;
        }


        [HttpPost]
        [Route("GetPriceByProductId{productId}")]
        public async Task<IActionResult> GetPriceById(int productId)
        {
            try
            {
                var price = await _priceService.GetPriceByProductIdAsync(productId);
                // create a varuable holding the price and Id of type ProductIdPriceDto
                var productIdPriceDto = new ProductIdPriceDto
                {
                    ProductId = productId,
                    ProductPrice = price
                };
                return Ok(productIdPriceDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Route("GetPricesByProductIds")]
        public async Task<IActionResult> GetPricesByProductIds([FromBody] ProductIdsRequestDto request)
        {
            try
            {
                var prices = new List<ProductIdPriceDto>();

                foreach (var productId in request.ProductIds)
                {
                    var price = await _priceService.GetPriceByProductIdAsync(productId);

                    prices.Add(new ProductIdPriceDto
                    {
                        ProductId = productId,
                        ProductPrice = price
                    });
                }

                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreatePrice(CreatePriceDto dto)
        {
            try
            {
                await _priceService.CreatePriceAsync(dto.ProductId, dto.ProductPrice);
                return CreatedAtAction(nameof(GetPriceById), new { productId = dto.ProductId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
