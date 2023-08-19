using NServiceBus;

namespace OrderManagement.Events
{
    public record OrderCreated: IEvent
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
    }
}