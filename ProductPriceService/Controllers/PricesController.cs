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

        //create an update api to be used from the ProductCatalog 
        [HttpPut]
        //Route with id
        [Route("UpdateProductPrice{productId:int}")]


        public async Task<IActionResult> UpdatePrice(UpdatePriceDto dto)
        {
            try
            {
                await _priceService.UpdatePriceAsync(dto.ProductId, dto.ProductPrice);
                //return Message that the price has been updated
                return Ok(dto);
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
        [Route("DeletePrice{productId:int}")]
        public async Task<IActionResult> DeletePrice(int productId)
        {
            try
            {
                await _priceService.DeletePriceAsync(productId);
                return Ok($"Product {productId} price has been deleted successfully!");
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
    }
}
