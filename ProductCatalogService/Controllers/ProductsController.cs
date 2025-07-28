
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.DTO;
using ProductCatalogService.Mapper;
using ProductCatalogService.Models;
using ProductCatalogService.Services;

namespace ProductCatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _productServices;


        public ProductsController(IProductServices productServices)
        {
            _productServices = productServices;

        }

        // GET: api/Products

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        { 

            try
            {
                var products = await _productServices.GetAllProductsAsync();
                
                



                return Ok(products);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        // GET: api/Products/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productServices.GetProductByIdAsync(id);
                //var productDto = ProductMapper.ToDto(product);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
        {

            var productDomain = CreateProductMapper.ToEntity(product);


            productDomain = await _productServices.CreateProductAsync(productDomain, product.ProductPrice);

            var productDto = ProductMapper.ToDto(productDomain);
            return CreatedAtAction(nameof(GetProductById), new { id = productDto.ProductId }, productDto);
        }

        // DELETE: api/Products/5
        [HttpDelete]
        [Route("{ProductId:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int ProductId)
        {
            try
            {
                var deletedProduct = await _productServices.DeleteProductAsync(ProductId);
                if (deletedProduct == null)
                {
                    return NotFound($"Product with ID {ProductId} not found.");
                }
                var productDto = ProductMapper.ToDto(deletedProduct);
                return Ok(deletedProduct);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/Products/5
        [HttpPut]
        //[Route("{id:int}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductIdPriceDto product)
        {
            try
            {
                var updatedProduct = await _productServices.UpdateProductPriceAsync(product.ProductId, product.ProductPrice);
                if (updatedProduct == null)
                {
                    return NotFound($"Product with ID {product.ProductId} not found.");
                }
                //var productDto = ProductMapper.ToDto(updatedProduct);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        //write a route with id
        [Route("UpdateName{ProductId:int}")]

        public async Task<IActionResult> UpdateProductName([FromRoute] int ProductId, [FromBody] UpdateProductNameDto newName)
        {
            try
            {
                var updatedName = await _productServices.UpdateProductNameAsync(ProductId, newName);

                if (updatedName == null)
                {
                    return NotFound($"Product with ID {ProductId} not found.");
                }

                return Ok(updatedName);
            }
            catch (Exception ex)
            {

                return NotFound(ex.Message);
            }
        }
    }


}
