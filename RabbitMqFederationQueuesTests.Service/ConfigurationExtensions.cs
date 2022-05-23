using MassTransit;
using RabbitMqFederationQueuesTests.Contracts;
using System.Reflection;


namespace RabbitMqFederationQueuesTests.Service;

public static class ConfigurationExtensions
{
    public static void AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConfig = new RabbitMqConfiguration();
        configuration.GetSection(RabbitMqConfiguration.SectionName).Bind(rabbitMqConfig);
        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.SectionName));

        services.AddMassTransit(busConfigurator =>
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(entryAssembly);
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                //cfg.DeployTopologyOnly = true;
                cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                 {
                     h.Username(rabbitMqConfig.User);
                     h.Password(rabbitMqConfig.Password);
                 });

                var queueName = ConfigurationConstants.GetInboxExchangeName(rabbitMqConfig.DatacenterId);
                cfg.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumer<CreateUserCommandConsumer>(context);
                });
            });

            ConfigureSendCommands(rabbitMqConfig);
        });
    }

    private static void ConfigureSendCommands(RabbitMqConfiguration rabbitMqConfig)
    {
        EndpointConvention.Map<CreateUserCommand>(new Uri($"exchange:{ConfigurationConstants.GetOutboundExchangeName(rabbitMqConfig.DatacenterId)}"));
    }

}
