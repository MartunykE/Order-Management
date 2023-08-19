using Microsoft.AspNetCore.Mvc.Filters;
using NServiceBus.TransactionalSession;
using Microsoft.Extensions.DependencyInjection;


namespace OrderManagement.Infrastructure.NServiceBus
{
    public class MessageSessionFilter : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (context.ActionDescriptor.Parameters.Any(p => p.ParameterType == typeof(ITransactionalSession)))
            {
                var session = context.HttpContext.RequestServices.GetRequiredService<ITransactionalSession>();
                await session.Open(new SqlPersistenceOpenSessionOptions());

                await next();

                await session.Commit();
            }
            else
            {
                await next();
            }
        }
    }
}
