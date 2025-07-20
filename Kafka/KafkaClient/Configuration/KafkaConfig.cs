namespace KafkaClient.Configuration;

public class KafkaConfig
{
    public required string BootstrapServers { get; set; }
    public required string Topic { get; set; }
    public int ConsumerGroupId { get; set; } = 1;
}