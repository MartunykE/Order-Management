using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.NServiceBus;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public async Task<IReadOnlyCollection<Order>> GetAll([FromServices] IOrdersService ordersService)
        {
            return await ordersService.GetAllOrders();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Order>> Get(int id, [FromServices] IOrdersService ordersService)
        {
            Order? order = await ordersService.GetOrder(id);

            if (order is null)
            {
                return NotFound();
            }

            return Ok(order);
        }
     
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDTO orderDTO, [FromServices] IOrdersService ordersService)
        {
            int id = await ordersService.CreateOrder(orderDTO);

            return Ok(id);
        }
    }
}
