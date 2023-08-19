using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Moq.EntityFrameworkCore;
using NServiceBus.TransactionalSession;
using OrderManagement.Application;
using OrderManagement.Application.DTO;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.EF;

namespace OrderManagement.Tests
{
    public class OrdersServiceTests
    {
        Mock<ITransactionalSession> transactionalSession;

        DbContextOptionsBuilder<OrderManagementDbContext> dbContextOptions;

        Mock<OrderManagementDbContext> orderManagementDbContext;
        OrdersService ordersService;

        public OrdersServiceTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<OrderManagementDbContext>();
            transactionalSession = new Mock<ITransactionalSession>();

            orderManagementDbContext = new Mock<OrderManagementDbContext>(dbContextOptions.Options);
            ordersService = new OrdersService(orderManagementDbContext.Object, transactionalSession.Object);
        }

        [Fact]
        public async void CreateOrder_WhenCustomerDoesNotExist_ThrowsApplicationException()
        {
            //Arrange 
            var createOrderDTO = new CreateOrderDTO(1, new List<int>());
            orderManagementDbContext.Setup(c => c.Customers).ReturnsDbSet(new List<Customer>());

            ///Assert
            await Assert.ThrowsAnyAsync<OrderManagement.Application.Exceptions.ApplicationException>(async () => await ordersService.CreateOrder(createOrderDTO));
        }


        [Fact]
        public async void CreateOrder_WhenProductDoesNotExist_ThrowsApplicationException()
        {
            //Arrange 
            var customerId = 1;
            var notExistedProductId = 1;
            var createOrderDTO = new CreateOrderDTO(customerId, new List<int>() { notExistedProductId });
            var customer = new Customer
            {
                Id = customerId,
                Name = "Test",
                OrdersCount = 0
            };
            customer.Id = customerId;

            var product = new Product
            {
                Id = 2,
                Name = "Test",
                Price = new Price { Currency = Currency.USD, Value = 2 },
            };

            orderManagementDbContext.Setup(c => c.Customers).ReturnsDbSet(new List<Customer> { customer });
            orderManagementDbContext.Setup(c => c.Products).ReturnsDbSet(new List<Product> { product });
            orderManagementDbContext.Setup(c => c.Orders).ReturnsDbSet(new List<Order>());

            ///Assert
            await Assert.ThrowsAnyAsync<OrderManagement.Application.Exceptions.ApplicationException>(async () => await ordersService.CreateOrder(createOrderDTO));
        }

        [Fact]
        public async void CreateOrder_WhenCustomerAndProductExists_CallsDbContextAdd()
        {
            //Arrange 
            var customerId = 1;
            var productId = 1;
            var createOrderDTO = new CreateOrderDTO(customerId, new List<int>() { productId });
            var customer = new Customer
            {
                Id = customerId,
                Name = "Test",
                OrdersCount = 0
            };
            customer.Id = customerId;

            var product = new Product
            {
                Id = productId,
                Name = "Test",
                Price = new Price { Currency = Currency.USD, Value = 2 },
            };

            orderManagementDbContext.Setup(c => c.Customers).ReturnsDbSet(new List<Customer> { customer });
            orderManagementDbContext.Setup(c => c.Products).ReturnsDbSet(new List<Product> { product });
            orderManagementDbContext.Setup(c => c.Orders).ReturnsDbSet(new List<Order>());

            var expectedOrderId = 1;

            //Act 
            var actualOrderId = await ordersService.CreateOrder(createOrderDTO);

            ///Assert
            orderManagementDbContext.Verify(c => c.Orders.Add(
                It.Is<Order>(o =>
                o.Customer.Id == customerId &&
                o.Products.FirstOrDefault(p => p.Id == productId) != null)
                ), Times.Once);
        }
    }
}
