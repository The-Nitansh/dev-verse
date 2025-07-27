using Confluent.Kafka;

namespace KafkaClient;

public static class QConfiguration
{
    public static ProducerConfig GetProducerConfig(KafkaProducerConfiguration kafkaProducerConfiguration)
    {
        return new ProducerConfig
        {
            BootstrapServers = kafkaProducerConfiguration.BootstrapServers,
            ClientId = kafkaProducerConfiguration.ClientId,
            Acks = (Acks)kafkaProducerConfiguration.Acks,
            CompressionType = (CompressionType)kafkaProducerConfiguration.CompressionType,
            MessageMaxBytes = kafkaProducerConfiguration.MessageMaxBytes,
            LingerMs = kafkaProducerConfiguration.LingerMs,
            BatchNumMessages = kafkaProducerConfiguration.BatchSize
        };
    }

    public static ConsumerConfig GetConsumerConfig(KafkaConsumerConfiguration kafkaConsumerConfiguration)
    {
        return new ConsumerConfig
        {
            BootstrapServers = kafkaConsumerConfiguration.BootstrapServers,
            ClientId = kafkaConsumerConfiguration.ClientId,
            //MessageMaxBytes = kafkaConsumerConfiguration.MessageMaxBytes,
            GroupId = kafkaConsumerConfiguration.GroupId,
            AutoOffsetReset = (AutoOffsetReset)kafkaConsumerConfiguration.AutoOffsetReset,
            //EnableAutoCommit = kafkaConsumerConfiguration.EnableAutoCommit,
            //AutoCommitIntervalMs = kafkaConsumerConfiguration.AutoCommitIntervalMs,
            //HeartbeatIntervalMs = kafkaConsumerConfiguration.HeartbeatIntervalMs,
            //SessionTimeoutMs = kafkaConsumerConfiguration.SessionTimeoutMs,
            //EnablePartitionEof = kafkaConsumerConfiguration.EnablePartitionEof
        };
    }
}