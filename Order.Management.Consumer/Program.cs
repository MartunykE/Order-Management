using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.TransactionalSession;
using OrderManagement.Application;
using OrderManagement.Infrastructure.EF.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomersService>();


string dbConnectionString = builder.Configuration.GetConnectionString("SqlServer");
string azureServiceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBusConnectionString");

builder.Services.AddEFDbContext(dbConnectionString);

builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("Order-Consumer");

    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>(new AzureServiceBusTransport(azureServiceBusConnectionString)
    {
        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
    });

    endpointConfiguration.EnableInstallers();


    var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    persistence.SqlDialect<SqlDialect.MsSqlServer>();
    persistence.ConnectionBuilder(() => new SqlConnection("Server=localhost;Database=OrderManagementDb;User ID=test2;Password=test1;TrustServerCertificate=True;Trusted_Connection=False;"));
    persistence.EnableTransactionalSession();

    endpointConfiguration.EnableOutbox();

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



app.Run();
