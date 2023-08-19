using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application;
using OrderManagement.Domain;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController:ControllerBase
    {
     
        [HttpGet]
        public async Task<IReadOnlyCollection<Product>> GetAll([FromServices] IProductsService productsService)
        {
            return await productsService.GetAllProducts();
        }

    }
}
