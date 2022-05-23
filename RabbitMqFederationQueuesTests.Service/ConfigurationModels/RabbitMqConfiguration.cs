namespace RabbitMqFederationQueuesTests.Service;

public record RabbitMqConfiguration
{
    public const string SectionName = "RabbitQm";
    public string Host { get; init; } = string.Empty;
    public ushort Port { get; init; }
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DatacenterId { get; init; } = string.Empty;
    public bool SendSampleMessage { get; init; } = true;
    public string[] AllDatacentersIds { get; init; } = Array.Empty<string>();
}
