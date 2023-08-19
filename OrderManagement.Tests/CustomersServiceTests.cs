using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using OrderManagement.Application;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.EF;

namespace OrderManagement.Tests
{
    public class CustomersServiceTests
    {
        DbContextOptionsBuilder<OrderManagementDbContext> dbContextOptions;
        Mock<OrderManagementDbContext> orderManagementDbContext;
        CustomersService customersService;

        public CustomersServiceTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<OrderManagementDbContext>();

            orderManagementDbContext = new Mock<OrderManagementDbContext>(dbContextOptions.Options);
            customersService = new CustomersService(orderManagementDbContext.Object);
        }

        [Fact]
        public async void UpdateOrdersCount_WhenCustomerDoesNotExist_ThrowsApplicationException()
        {
            //Arrange
            orderManagementDbContext.Setup<DbSet<Customer>>(c => c.Customers)
                .ReturnsDbSet(new List<Customer>());

            //Assert
            await Assert.ThrowsAnyAsync<OrderManagement.Application.Exceptions.ApplicationException>(async () => await customersService.UpdateOrdersCount(1));
        }

        [Fact]
        public async void UpdateOrdersCount_WhenCustomerExist_SetsOrdersCount()
        {
            //Arrange
            var customerId = 1;
            var orders = new List<Order>();
            var customer = new Customer
            {
                Id = customerId,
                OrdersCount = 0,
                Orders = orders,
                Name = "test"
            };
            var order = new Order
            {
                Customer = customer,
                CustomerId = customerId,
                Products = new List<Product>(),
                Id = 2
            };
            orders.Add(order);

            var expectedOrdersCount = 1;

            orderManagementDbContext.Setup<DbSet<Customer>>(c => c.Customers)
                .ReturnsDbSet(new List<Customer>() { customer });

            orderManagementDbContext.Setup<DbSet<Order>>(c => c.Orders)
                .ReturnsDbSet(orders);

            //Act
            await customersService.UpdateOrdersCount(customerId);

            //Assert
            Assert.Equal(expectedOrdersCount, customer.OrdersCount);
        }
    }
}
