using RabbitMqFederationQueuesTests.Service;

Console.Title = "Rabbit MQ Federation Queues Example";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IObjectsRepository, InMemoryObjectsRepository>();
        services.AddCustomMassTransit(hostContext.Configuration);
        services.AddHostedService<DeployFederationTopologyBackgroundService>();
        services.AddHostedService<CommandsPublisherBackgroundService>();
    })
    .Build();

await host.RunAsync();
