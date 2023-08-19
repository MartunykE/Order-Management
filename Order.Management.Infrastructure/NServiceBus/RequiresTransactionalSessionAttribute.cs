using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NServiceBus.TransactionalSession;


namespace OrderManagement.Infrastructure.NServiceBus
{
    public class RequiresTransactionalSessionAttribute : TypeFilterAttribute
    {
        public RequiresTransactionalSessionAttribute() : base(typeof(TransactionalSessionFilter))
        {
        }
    }
}

public class TransactionalSessionFilter : IAsyncResourceFilter
{
    private readonly ITransactionalSession transactionalSession;

    public TransactionalSessionFilter(ITransactionalSession transactionalSession)
    {
        this.transactionalSession = transactionalSession;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        await transactionalSession.Open(new SqlPersistenceOpenSessionOptions());
        try
        {

            await next();
            await transactionalSession.Commit();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
