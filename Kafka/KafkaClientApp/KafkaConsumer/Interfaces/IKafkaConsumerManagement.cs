using Confluent.Kafka;
using KafkaConsumer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaConsumer.Interfaces;
interface IKafkaConsumerManagement {}


public interface IDLQProducer
{
    Task SendToDlqAsync(DLQMessage message, CancellationToken cancellationToken = default);
}

public interface IConsumerServiceHandler
{
    Task StartConsumingAsync(CancellationToken cancellationToken);
}

public delegate IConsumerServiceHandler KafkaConsumerHandlerFactory(int consumerId);

public interface IErrorPolicy
{
    Task HandleAsync(ConsumeResult<Ignore, string> input, Exception exception, CancellationToken token);
} 
