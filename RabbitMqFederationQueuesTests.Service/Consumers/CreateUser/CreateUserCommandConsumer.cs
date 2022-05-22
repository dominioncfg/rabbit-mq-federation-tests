using MassTransit;
using RabbitMqFederationQueuesTests.Contracts;

namespace RabbitMqFederationQueuesTests.Service;

public class CreateUserCommandConsumer : IConsumer<CreateUserCommand>
{
    readonly ILogger<CreateUserCommandConsumer> _logger;
    private readonly IObjectsRepository _objectsRepository;

    public CreateUserCommandConsumer(ILogger<CreateUserCommandConsumer> logger, IObjectsRepository objectsRepository)
    {
        _logger = logger;
        _objectsRepository = objectsRepository;
    }

    public async Task Consume(ConsumeContext<CreateUserCommand> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message);
        _objectsRepository.Add(context.Message);
        var random = new Random();
        var delayFor = random.Next(200, 1000);
        await Task.Delay(delayFor);
    }
}
