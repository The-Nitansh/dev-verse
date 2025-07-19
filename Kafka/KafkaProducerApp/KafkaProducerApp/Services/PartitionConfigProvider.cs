using KafkaProducerApp.DataModels;

namespace KafkaProducerApp.Services;

public class PartitionConfigProvider
{
    public List<PartitionMapping> GetPartitionMappings()
    {
        return new List<PartitionMapping>
        {
            new PartitionMapping
            {
                UtilityName = "UtilityA",
                Partitions = new List<int> { 0, 1, 2 }
            },
            new PartitionMapping
            {
                UtilityName = "UtilityB",
                Partitions = new List<int> { 3, 4 }
            },
            new PartitionMapping
            {
                UtilityName = "UtilityC",
                Partitions = new List<int> { 5 }
            }
        };
    }
}
