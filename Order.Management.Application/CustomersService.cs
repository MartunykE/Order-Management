using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.EF;

namespace OrderManagement.Application
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomer(int id);
        
        Task<int> CreateCustomer(CreateCustomerDTO customerDTO);
        
        Task UpdateOrdersCount(int customerId);

    }

    public class CustomersService : ICustomerService
    {
        private readonly OrderManagementDbContext dbContext;

        public CustomersService(OrderManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CreateCustomer(CreateCustomerDTO customerDTO)
        {
            Customer customer = new Customer
            {
                Name = customerDTO.Name
            };

            dbContext.Customers.Add(customer);

            await dbContext.SaveChangesAsync();

            return customer.Id;
        }

        public async Task<Customer?> GetCustomer(int id)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateOrdersCount(int customerId)
        {
            Customer? customer = await dbContext.Customers
                .FirstOrDefaultAsync(c =>  c.Id == customerId);   

            if (customer is null)
            {
                throw new Exceptions.ApplicationException($"Customer with id: {customerId} was not found ");
            }

            int ordersCount = await  dbContext.Orders
                .CountAsync(o => o.CustomerId == customerId);

            customer.OrdersCount = ordersCount;
            await dbContext.SaveChangesAsync();
        }
    }
}
