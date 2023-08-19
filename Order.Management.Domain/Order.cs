namespace OrderManagement.Domain
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public required Customer Customer { get; set; }

        public required List<Product> Products { get; set; } 

    }
}
