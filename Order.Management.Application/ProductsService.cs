using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain;
using OrderManagement.Infrastructure.EF;

namespace OrderManagement.Application
{
    public interface IProductsService
    {
        Task<IReadOnlyCollection<Product>> GetAllProducts();

    }

    public class ProductsService : IProductsService
    {
        private readonly OrderManagementDbContext dbContext;

        public ProductsService(OrderManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllProducts()
        {
            return await dbContext.Products.ToListAsync();
        }

    }
}
