using MassTransit;
using RabbitMqFederationQueuesTests.Contracts;
using System.Reflection;


namespace RabbitMqFederationQueuesTests.Service;

public static class ConfigurationExtensions
{
    public static void AddCustomMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(busConfigurator =>
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(entryAssembly);
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(ConfigurationConstants.WorkerQueueName, e =>
                {
                    e.ConfigureConsumer<CreateUserCommandConsumer>(context);
                });
            });

            ConfigureSendCommands();
        });
    }

    private static void ConfigureSendCommands()
    {
        EndpointConvention.Map<CreateUserCommand>(new Uri($"queue:{ConfigurationConstants.WorkerQueueName}"));
    }  
}
