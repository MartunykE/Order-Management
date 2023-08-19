using OrderManagement.Domain;

namespace OrderManagement.Infrastructure.EF
{
    public static class DbInitializer
    {
        public static void Initialize(OrderManagementDbContext context)
        {

            if (context.Products.Any())
            {
                return;
            }

            List<Product> products = new List<Product>
            {
                new Product
                {
                    Name = "Phone",
                    Price = new Price
                    {
                        Currency = Currency.USD,
                        Value = 500
                    }
                },
                new Product
                {
                    Name = "Laptop",
                    Price = new Price
                    {
                        Currency = Currency.USD,
                        Value = 1000
                    }
                },
                new Product
                {
                    Name = "Keyboard",
                    Price = new Price
                    {
                        Currency = Currency.EUR,
                        Value = 20
                    }
                }
            };

            context.Products.AddRange(products);

            context.SaveChanges();


        }
    }
}
