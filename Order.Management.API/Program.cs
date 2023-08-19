using OrderManagement.Application;
using NServiceBus;
using NServiceBus.TransactionalSession;
using Microsoft.Data.SqlClient;
using OrderManagement.Events;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.EF;
using OrderManagement.Infrastructure.EF.Extensions;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.NServiceBus;
using OrderManagement.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ICustomerService, CustomersService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddScoped<RequiresTransactionalSessionAttribute>();


string dbConnectionString = builder.Configuration.GetConnectionString("SqlServer");
string azureServiceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBusConnectionString");

builder.Services.AddEFDbContext(dbConnectionString);

builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("Order-Sender");

    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>(new AzureServiceBusTransport(azureServiceBusConnectionString)
    {
        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
    });


    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    persistence.SqlDialect<SqlDialect.MsSqlServer>();
    persistence.ConnectionBuilder(() => new SqlConnection(dbConnectionString));
    persistence.EnableTransactionalSession();

    endpointConfiguration.EnableInstallers();
    endpointConfiguration.EnableOutbox();

    transport.RouteToEndpoint(assembly: typeof(OrderCreated).Assembly,
        destination: "Order-Consumer");

    return endpointConfiguration;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    ITransactionalSession? session = context.RequestServices.GetService<ITransactionalSession>();
    if (session is not null)
    {
        await session.Open(new SqlPersistenceOpenSessionOptions());
        await next();

        await session.Commit();
    }
    else
    {
        await next();
    }
});

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<OrderManagementDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}


app.Run();
