using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMqFederationQueuesTests.Service;

public class DeployFederationTopologyBackgroundService : BackgroundService
{
    private readonly IBusControl _busControl;
    private readonly RabbitMqConfiguration _appConfig;

    public DeployFederationTopologyBackgroundService(IBusControl busControl, IOptions<RabbitMqConfiguration> appConfig)
    {

        _busControl = busControl;
        _appConfig = appConfig.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _busControl.DeployAsync(cancellationToken);
        await _busControl.StartAsync(cancellationToken);
        ConfigureFederation();
    }

    private void ConfigureFederation()
    {
        var rabbitFactory = new ConnectionFactory()
        {
            HostName = _appConfig.Host,
            UserName = _appConfig.User,
            Password = _appConfig.Password,
            Port = _appConfig.Port,
        };
        var currentDatacenterInboxName = ConfigurationConstants.GetInboxExchangeName(_appConfig.DatacenterId);

        //****Exchanges and bindings
        using var connection = rabbitFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var externalDatacenters = _appConfig.AllDatacentersIds
            .Where(x => x != _appConfig.DatacenterId)
            .ToArray();

        //**Create a copy of extenal datacenters in This datacenter and BindidTo
        foreach (var item in externalDatacenters)
        {
            var copyExchangeName = ConfigurationConstants.GetOutboundExchangeName(item);
            channel.ExchangeDeclare(copyExchangeName, ExchangeType.Fanout, true, false);
            channel.ExchangeBind(currentDatacenterInboxName, copyExchangeName, "");
        }
    }
}

