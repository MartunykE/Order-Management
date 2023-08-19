Setup:

1. Set connection string to sql server and azure service bus in appsettings.json files in Order.Management.Consumer and OrderManagement.API projetcs
2. Build and run
3. Create customer using POST Customers endpoint
4. Create Order using POST Orders endpoint (Use Customer Id returned from step 3. List of avaliable products can be retrieved from GET Products endpoint)

Overview:

N-tier architecture was selected. That allowed to reuse application layer services OrderManagement.API and  Order.Management.Consumer; Alternatively CQS\CQRS coudl be used.
Mapping from Domain model to response models \ DTO`s was ommited in order to make tesing easier;
Used NServiceBus as an abstraction over Azure Service Bus. Used Outbox pattern for eventual consistency;
