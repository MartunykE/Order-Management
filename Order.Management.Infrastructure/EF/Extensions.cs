using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence.Sql;
using NServiceBus.Persistence;

namespace OrderManagement.Infrastructure.EF.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddEFDbContext(this IServiceCollection services, string dbConnectionString)
        {
            services.AddScoped(b =>
             {

                 if (b.GetService<ISynchronizedStorageSession>() is ISqlStorageSession { Connection: not null } session)
                 {
                     var dbContext = new OrderManagementDbContext(new DbContextOptionsBuilder<OrderManagementDbContext>()
                         .UseSqlServer(session.Connection)
                         .Options);

                     dbContext.Database.UseTransaction(session.Transaction);

                     session.OnSaveChanges((s, ct) => dbContext.SaveChangesAsync(ct));
                     return dbContext;

                 }
                 else
                 {
                     var dbContext = new OrderManagementDbContext(new DbContextOptionsBuilder<OrderManagementDbContext>()
                         .UseSqlServer(dbConnectionString)
                         .Options);
                     return dbContext;
                 }
             });

            return services;
        }
    }
}
