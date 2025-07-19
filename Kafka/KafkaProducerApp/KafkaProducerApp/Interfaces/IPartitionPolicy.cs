namespace KafkaProducerApp.Interfaces;

public interface IPartitionPolicy
{
    int ResolvePartition(string utilityName, string messageKey);
}
