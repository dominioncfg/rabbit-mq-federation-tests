using MassTransit;

namespace RabbitMqFederationQueuesTests.Service;

public class CreateUserCommandConsumerDefinition : ConsumerDefinition<CreateUserCommandConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateUserCommandConsumer> consumerConfigurator)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
