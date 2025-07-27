using Confluent.Kafka;

namespace KafkaClient;

public class KafkaConsumerConfiguration
{
    public required string BootstrapServers { get; set; }
    public required string Topic { get; set; }
    public string GroupId { get; set; }
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;
    //public bool IsConsumerEnabled { get; set; }
    public string ClientId { get; set; }
    //public bool EnableAutoCommit { get; set; }
    //public int AutoCommitIntervalMs { get; set; }
   // public int SessionTimeoutMs { get; set; }
    //public int HeartbeatIntervalMs { get; set; } = 100;
    //public int MessageMaxBytes { get; set; } = 10000;
    //public bool EnablePartitionEof { get; set; }
    //public bool IsConsumerPollingNotRequired { get; set; }
}

public class KafkaProducerConfiguration
{
    public bool IsProducerEnabled { get; set; }
    public string BootstrapServers { get; set; }
    public string ClientId { get; set; }
    public int Acks { get; set; }   // -1 = All, 0 = None, 1 = Leader
    public int CompressionType { get; set; }     // 0 = none, 1 = gzip, 2 = snappy, 3 = lz4, 4 = zstd
    public int MessageMaxBytes { get; set; }
    public int LingerMs { get; set; }
    public int BatchSize { get; set; }
}

