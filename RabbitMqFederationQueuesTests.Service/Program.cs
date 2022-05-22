using RabbitMqFederationQueuesTests.Service;

Console.Title = "Rabbit MQ Federation Queues Example";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IObjectsRepository, InMemoryObjectsRepository>();
        services.AddCustomMassTransit();
        services.AddHostedService<CommandsPublisherBackgroundService>();
    })
    .Build();

await host.RunAsync();
