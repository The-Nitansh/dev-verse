namespace KafkaProducerApp.Interfaces;

interface IKafkaProducerService
{
    Task ProduceMessageAsync(string utilityName, string message);
}