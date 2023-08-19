
namespace OrderManagement.Domain
{
    public class Product
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }

        public required Price Price { get; set; }
    }

    public class Price
    {
        public required Currency Currency { get; set; }

        public decimal Value { get; set; }
    }

    public enum Currency
    {
        USD,
        EUR
    }
}
