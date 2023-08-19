using Microsoft.EntityFrameworkCore;
using NServiceBus.TransactionalSession;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Events;
using OrderManagement.Infrastructure.EF;


namespace OrderManagement.Application
{
    public interface IOrdersService
    {
        Task<IReadOnlyCollection<Order>> GetAllOrders();
        Task<Order?> GetOrder(int id);
        Task<int> CreateOrder(CreateOrderDTO orderDTO);
    }

    public class OrdersService : IOrdersService
    {
        private readonly OrderManagementDbContext dbContext;
        private readonly ITransactionalSession transactionalSession;

        public OrdersService(OrderManagementDbContext dbContext, ITransactionalSession transactionalSession)
        {
            this.dbContext = dbContext;
            this.transactionalSession = transactionalSession;
        }


        public async Task<IReadOnlyCollection<Order>> GetAllOrders()
        {
            return await dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Products)
                .ToListAsync();
        }

        public async Task<Order?> GetOrder(int id)
        {
            return await dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> CreateOrder(CreateOrderDTO orderDTO)
        {
            Customer? customer = await dbContext.Customers
                .FirstOrDefaultAsync(o => o.Id == orderDTO.CustomerId);

            if (customer == null)
            {
                throw new Exceptions.ApplicationException($"Customer with id: {orderDTO.CustomerId} was not found ");
            }

            List<Product> products = await dbContext.Products
                .Where(p => orderDTO.ProductIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != orderDTO.ProductIds.Count)
            {
                IEnumerable<int> notFoundProductsIds = orderDTO.ProductIds
                    .Except(products.Select(c => c.Id));

                throw new Exceptions.ApplicationException($"Products with ids: {string.Join(',', notFoundProductsIds)} were not found ");
            }

            Order order = new Order
            {
                Customer = customer,
                Products = products,
            };
            OrderCreated @event = new OrderCreated
            {
                CustomerId = order.Customer.Id,
                OrderId = order.Id,
            };

            dbContext.Orders.Add(order);

            await transactionalSession.Publish(@event);

            await dbContext.SaveChangesAsync();
            return order.Id;
        }


    }
}
