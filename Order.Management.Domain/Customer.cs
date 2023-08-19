namespace OrderManagement.Domain
{
    public class Customer
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int OrdersCount { get; set; }

        public List<Order>? Orders { get; set; }

    }
}