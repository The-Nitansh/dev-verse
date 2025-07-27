/*using KafkaApp;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
*/


using KafkaApp.DataModels;
using KafkaApp.Services;
using KafkaClient;
using KafkaClient.Avro;
using KafkaClient.Interfaces;
using KafkaClient.Json;
using Microsoft.Extensions.Options;
using Serilog;

try
{
    IHost appHost = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        })
        .UseSerilog((context, services, config) =>
        {
            // automatically write down the all the available sinks
            var logConfig = config.ReadFrom.Configuration(context.Configuration)
                            .Enrich.FromLogContext();

        })
        .ConfigureServices((context, services) =>
        {
            IConfigurationSection kafkaConfig = context.Configuration.GetSection("KafkaConfig");
            bool flag = int.TryParse(kafkaConfig["ConsumerCount"], out var count);
            if (flag) { Global.ConsumerCount = count; }
            Global.ConsumerType = kafkaConfig["ConsumerType"] ?? "json";



            // DI
            services.Configure<KafkaConsumerConfiguration>(context.Configuration.GetSection("KafkaConsumerConfiguration") ?? throw new ArgumentNullException("KafkaConsumerConfiguration not found"));

            services.AddHostedService<ConsumerHostedService>();
            /*services.AddTransient<IKafkaConsumer<ReturnCode>, JsonKafkaConsumer<ReturnCode>>();
            services.AddTransient<IKafkaConsumer<ReturnCode>, AvroKafkaConsumer<ReturnCode>>();*/

        })
        .Build();

    appHost.Run();
}
catch (Exception e)
{
    Console.WriteLine($"An error occurred: {e.Message}");
}
