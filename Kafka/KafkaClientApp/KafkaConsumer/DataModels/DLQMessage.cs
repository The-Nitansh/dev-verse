namespace KafkaConsumer.DataModels;

public class DLQMessage
{
    public string Message { get; set; }
    public string Reason { get; set; }
    public string Topic { get; set; }
    public int ConsumerId { get; set; }
    public int Partition { get; set; }
    public long Offset { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

