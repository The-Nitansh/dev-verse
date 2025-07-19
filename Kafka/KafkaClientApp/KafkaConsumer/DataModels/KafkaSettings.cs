namespace KafkaConsumer.DataModels;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public string Topic { get; set; }
    public string DlqTopic { get; set; }
    public bool EnableAutoCommit { get; set; }
    public string AutoOffsetReset { get; set; }
}

class KafkaConfig
{
    public int? ConsumerCount { get; set; } = 2; // Default to 2 consumers if not set
}
