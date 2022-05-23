namespace RabbitMqFederationQueuesTests.Service;

public class ConfigurationConstants
{
    public const string InboxQueuesSuffix = "inbox";
    public const string OutboundQueuesSuffix = "outbound";


    public static string GetOutboundExchangeName(string datacenterId)
    {
        return $"{datacenterId}-{OutboundQueuesSuffix}";
    }

    public static string GetInboxExchangeName(string datacenterId)
    {
        return $"{datacenterId}-{InboxQueuesSuffix}";
    }
}
