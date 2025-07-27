using Confluent.Kafka;
using KafkaApp.DataModels;
using KafkaApp.Logging;
using KafkaClient;
using KafkaClient.Avro;
using KafkaClient.Factory;
using KafkaClient.Interfaces;
using KafkaClient.Json;
using Microsoft.Extensions.Options;

namespace KafkaApp.Services;

public class ConsumerHostedService : BackgroundService
{
    private readonly KafkaConsumerConfiguration _config;
    private readonly List<Task> _tasks = new();
    //private readonly ILoggerFactory _loggerFactory;
    private ILogger<ConsumerHostedService> logger;
    private ILogger<JsonKafkaConsumer<ReturnCode>> jsonLogger;
    private ILogger<AvroKafkaConsumer<ReturnCode>> avroLogger;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, ILoggerFactory loggerFactory, IOptions<KafkaConsumerConfiguration> config)
    {
        _config = config.Value;
        //_loggerFactory = loggerFactory;
        this.logger = logger;
        InitLoggers(loggerFactory);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (int i = 1; i <= Global.ConsumerCount; i++)
        {
            var consumerId = $"Consumer-{i}";

            IKafkaConsumer<ReturnCode> consumer = Global.ConsumerType switch
            {
                "json" => KafkaClientFactory<ReturnCode, object>.BuildJsonConsumer(jsonLogger, _config, consumerId, message =>
                {
                    Console.WriteLine($"{consumerId}: {message}");
                    //await Task.CompletedTask;
                    return ReturnCode.Success;
                }),

                "avro" => KafkaClientFactory<ReturnCode, object>.BuildAvroConsumer(avroLogger, _config, consumerId, message =>
                {
                    Console.WriteLine($"{consumerId}: {message}");
                    //await Task.CompletedTask;
                    return ReturnCode.Success;
                }),

                _ => throw new InvalidOperationException("Unsupported consumer format.")
            };

            consumer.StartConsuming(stoppingToken);
        }

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_tasks);
    }

    private void InitLoggers(ILoggerFactory loggerFactory)
    {
        CustomLoggerFactory.Configure(loggerFactory);
        jsonLogger = CustomLoggerFactory.For<JsonKafkaConsumer<ReturnCode>>();
        avroLogger = CustomLoggerFactory.For<AvroKafkaConsumer<ReturnCode>>();
    }
}
