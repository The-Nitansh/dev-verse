using Confluent.Kafka;
using KafkaConsumer.DataModels;
using KafkaConsumer.Interfaces;
using KafkaConsumer.Transformer;
using KafkaConsumer.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace KafkaConsumer.Services;

public class ConsumerServiceHandler: IConsumerServiceHandler
{
    private readonly ILogger<ConsumerServiceHandler> logger;
    private readonly IDummyService dummyServiceHandler;
    private readonly IDLQProducer dlqProducer;
    private readonly DummyInputHandler inputHandler;
    private readonly int consumerId;
    private readonly KafkaSettings kafkaSettings;

    public ConsumerServiceHandler(ILogger<ConsumerServiceHandler> logger, IOptions<KafkaSettings> kafkaSettingOptions, IDummyService dummyServiceHandler, IDLQProducer dlqProducer, DummyInputHandler inputHandler, int consumerId)
    {
        this.logger = logger;
        this.dummyServiceHandler = dummyServiceHandler;
        this.dlqProducer = dlqProducer;
        this.inputHandler = inputHandler;
        this.consumerId = consumerId;
        kafkaSettings= kafkaSettingOptions.Value;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            GroupId = kafkaSettings.GroupId,
            EnableAutoCommit = kafkaSettings.EnableAutoCommit,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(kafkaSettings.AutoOffsetReset, ignoreCase: true)
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(kafkaSettings.Topic);
        logger.LogInformation("[ORG>Consumer-{consumerId}] Subscribed to topic: {topic}", consumerId, kafkaSettings.Topic);
        string message = string.Empty;
        int partition = -1;
        long offset = -1;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string> result = consumer.Consume(cancellationToken);
                // just for logging
                {
                    partition = result.Partition.Value;
                    offset = result.Offset.Value;
                    message = result.Message.Value;
                }
                var status = inputHandler.CreateAndValidateDummyModelAsync(message, cancellationToken, out DummyModel? dummyModel);
                if (status == ReturnCode.Failure){ continue; }

                logger.LogInformation($"[ORG>Consumer-{consumerId}] Received: {result.Message.Value} | Partition: {result.Partition} | Offset: {result.Offset}");
                try
                {
                    status = dummyServiceHandler.DummyProcessor(dummyModel!, out DummyOutput output);
                    if(status != ReturnCode.Success)
                    {
                        var dlqMessage = new DLQMessage
                        {
                            ConsumerId = consumerId,
                            Topic = kafkaSettings.Topic,
                            Partition = partition,
                            Offset = offset,
                            Reason = output.Error,
                            Timestamp = DateTime.UtcNow,
                            Message = message
                        };

                        dlqProducer.SendToDlqAsync(dlqMessage, cancellationToken)
                            .GetAwaiter()
                            .GetResult();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
                consumer.Commit(result);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[Consumer-{consumerId}] Cancelled", consumerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Consumer-{consumerId}] Unexpected error: {message}", consumerId, ex.Message);
        }
        finally
        {
            if(cancellationToken.IsCancellationRequested)
            {
                consumer.Close();
                logger.LogInformation("[Consumer-{consumerId}] Clo", consumerId);
            }
        }
    }
}

