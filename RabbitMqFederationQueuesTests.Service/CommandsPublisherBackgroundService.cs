using MassTransit;
using Microsoft.Extensions.Options;

namespace RabbitMqFederationQueuesTests.Service;

public class CommandsPublisherBackgroundService : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<CommandsPublisherBackgroundService> _logger;
    private readonly RabbitMqConfiguration _appConfig;

    public CommandsPublisherBackgroundService(IBus bus, ILogger<CommandsPublisherBackgroundService> logger, IOptions<RabbitMqConfiguration> appConfig)
    {
        
        _bus = bus;
        _logger = logger;
        _appConfig = appConfig.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await WaitUntilRabbitHasStarted();

        if(_appConfig.SendSampleMessage)
        {
            await SendMessagesUnlessStopped(cancellationToken);
        }        
    }

    private static async Task WaitUntilRabbitHasStarted()
    {
        await Task.Delay(5000);
    }

    private async Task SendMessagesUnlessStopped(CancellationToken cancellationToken)
    {
        var r = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            var durationInSeconds = r.Next(3, 11);
            var numberOfMessagePerSecond = 1;
            await SendMessagesAtGivenCadence(durationInSeconds, numberOfMessagePerSecond, cancellationToken);
        }
    }

    protected async Task SendMessagesAtGivenCadence(int durationInSeconds, int messagesPerSecond, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending {NumberOfMessage} messages per second during {Duration} seconds", messagesPerSecond, durationInSeconds);

        var random = new Random();
        var initialTime = DateTime.Now;
        while (!cancellationToken.IsCancellationRequested && (DateTime.Now - initialTime).Seconds <= durationInSeconds)
        {
            await SendCreateUserCommand(random, cancellationToken);

            var delay = 1000 / messagesPerSecond;
            await Task.Delay(delay, cancellationToken);
        }
    }

    private async Task SendCreateUserCommand(Random random, CancellationToken cancellationToken)
    {
        var entrophy = random.Next();
        var message = new Contracts.CreateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = $"FN {entrophy}",
            LastName = $"LN {entrophy} from Datacenter {_appConfig.DatacenterId}",
        };

        await _bus.Send(message, cancellationToken);
        _logger.LogInformation("Sended message of type {MessageType} with content{Message}", message.GetType().Name, message);
    }

}

