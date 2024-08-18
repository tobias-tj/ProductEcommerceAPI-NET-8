using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPIApplication.DTOs;
using ProductAPIApplication.DTOs.Conversions;
using ProductAPIApplication.Interfaces;

namespace ProductAPIPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct _productInterface;
        public ProductsController(IProduct productInterface)
        {
            _productInterface = productInterface;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            // Get all products from repo
            var products = await _productInterface.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No product found!");
            }

            // conver data from entity to DTO and return
            var (_, list) = ProductConversion.FromEntity(null!, products);

            return list!.Any() ? Ok(list) : NotFound("No product found");

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            // Get single product from Repo
            var product = await _productInterface.FindByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product requested not found");
            }
            // convert from entity to Dto and return
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("Product not found");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // convert to Entity 
            var getEntity = ProductConversion.ToEntity(product);
            var response = await _productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await _productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await _productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

    }
}
