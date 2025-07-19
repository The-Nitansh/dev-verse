using System.Diagnostics.CodeAnalysis;

namespace KafkaConsumer.DataModels;

public class DummyModel
{
    public int Id { get; set; }
    public string? Description { get; set; }
}

public class DummyOutput
{
    public int ConsumerId { get; set; }
    public string Result { get; set; }
    public string Error { get; set; }
}
