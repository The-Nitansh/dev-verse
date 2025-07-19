namespace KafkaProducerApp.DataModels;

public class PartitionMapping
{
    public string UtilityName { get; set; }
    public List<int> Partitions { get; set; }
}
