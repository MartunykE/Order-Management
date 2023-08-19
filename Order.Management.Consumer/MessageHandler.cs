using NServiceBus;
using OrderManagement.Application;
using OrderManagement.Events;

namespace Order.Management.Consumer
{
    public class MessageHandler : IHandleMessages<OrderCreated>
    {
        private readonly ICustomerService customerService;

        public MessageHandler(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public async Task Handle(OrderCreated message, IMessageHandlerContext context)
        {
            await customerService.UpdateOrdersCount(message.CustomerId);
        }
    }
}
