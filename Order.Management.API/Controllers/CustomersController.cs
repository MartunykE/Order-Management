using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.NServiceBus;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController: ControllerBase
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Customer>> Get(int id, [FromServices] ICustomerService customerService)
        {
            Customer? order = await customerService.GetCustomer(id);

            if (order is null)
            {
                return NotFound();
            }

            return Ok(order);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDTO customerDTO, [FromServices] ICustomerService customerService)
        {
            int id = await customerService.CreateCustomer(customerDTO);

            return Ok(id);
        }
    }
}
