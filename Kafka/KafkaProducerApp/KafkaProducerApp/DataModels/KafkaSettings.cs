namespace KafkaProducerApp.DataModels;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string Topic { get; set; }
    public int TotalPartitions { get; set; }
    public List<Utility> Utilities { get; set; }
}

public class Utility
{
    public string Name { get; set; }
    public List<int> Partitions { get; set; }
}
