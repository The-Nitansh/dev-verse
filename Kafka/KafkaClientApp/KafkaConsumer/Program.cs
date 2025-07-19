/*using KafkaConsumer;
using KafkaConsumer.DataModels;
using KafkaConsumer.Interfaces;
using KafkaConsumer.Services;
using KafkaConsumer.Transformer;
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
            var logConfig = config.ReadFrom.Configuration(context.Configuration)
                  .Enrich.FromLogContext();
        })
        .ConfigureServices((context, services) =>
        {
            // Bind KafkaSettings and KafkaConfig via IOptions pattern
            services.Configure<KafkaSettings>(context.Configuration.GetSection("KafkaSettings"));
            try
            {
                Global.ConsumerCount = context.Configuration.GetSection("KafkaConfig").Get<KafkaConfig>().ConsumerCount ?? 2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            services.AddTransient<IDummyService, DummyService>();
            services.AddTransient<IDLQProducer, DLQProducer>();
            services.AddTransient<DummyInputHandler>();
            services.AddTransient(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<DummyInputHandler>>();
                return new DummyInputHandler(msg => logger.LogError(msg));
            });

            // Register factory for KafkaConsumerServiceHandler with dynamic consumerId
            services.AddTransient<KafkaConsumerHandlerFactory>(serviceProvider =>
            {
                return consumerId =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<ConsumerServiceHandler>>();
                    var kafkaSettings = serviceProvider.GetRequiredService<IOptions<KafkaSettings>>();
                    var serviceHandler = serviceProvider.GetRequiredService<IDummyService>();
                    var inputHandler = serviceProvider.GetRequiredService<DummyInputHandler>();
                    var dlqProducer = serviceProvider.GetRequiredService<IDLQProducer>();

                    return new ConsumerServiceHandler(logger, kafkaSettings,serviceHandler, dlqProducer, inputHandler, consumerId);
                };
            });


            services.AddSingleton<ConsumerCoordinator>();
            services.AddHostedService<Worker>();
        })
        .Build();

    appHost.Run();
}
catch (Exception e)
{
    Console.WriteLine($"An error occurred: {e.Message}");
}*/

using KafkaConsumer;
using KafkaConsumer.Interfaces;
using KafkaConsumer.Services;
using KafkaConsumer.DataModels;
using Microsoft.Extensions.Options;
using Serilog;
using KafkaConsumer.Transformer;

try
{
     IHost appHost = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.Sources.Clear();
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Bind KafkaSettings via IOptions
        services.Configure<KafkaSettings>(context.Configuration.GetSection("KafkaSettings"));
    
        // Safely initialize global config
        try
        {
            Global.ConsumerCount = context.Configuration
                .GetSection("KafkaConfig")
                .Get<KafkaConfig>()?.ConsumerCount ?? 2;
        }
        catch (Exception ex)
        {
            Log.Error($"[Config Error] {ex.Message}");
            Log.Information("Using default ConsumerCount of 2");
        }
    
        // SCOPED services for per-consumer instance
        services.AddScoped<IDummyService, DummyService>();
        services.AddScoped<IDLQProducer, DLQProducer>();
        services.AddScoped(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DummyInputHandler>>();
            return new DummyInputHandler(msg => logger.LogError(msg));
        });
    
        // Factory delegate to resolve ConsumerServiceHandler per consumerId using scoped lifetime
        services.AddSingleton<KafkaConsumerHandlerFactory>(serviceProvider =>
        {
            return consumerId =>
            {
                // Create a new scope for this consumer instance
                var scope = serviceProvider.CreateScope();
                var scopedProvider = scope.ServiceProvider;
    
                var logger = scopedProvider.GetRequiredService<ILogger<ConsumerServiceHandler>>();
                var kafkaSettings = scopedProvider.GetRequiredService<IOptions<KafkaSettings>>();
                var serviceHandler = scopedProvider.GetRequiredService<IDummyService>();
                var inputHandler = scopedProvider.GetRequiredService<DummyInputHandler>();
                var dlqProducer = scopedProvider.GetRequiredService<IDLQProducer>();
    
                return new ConsumerServiceHandler(logger, kafkaSettings, serviceHandler, dlqProducer, inputHandler, consumerId);
            };
        });
    
        // Singleton coordinator and hosted background worker
        services.AddSingleton<ConsumerCoordinator>();
        services.AddHostedService<Worker>();
    })
    .UseSerilog((context, services, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
    })
    .Build();
    
     await appHost.RunAsync();

}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while starting the Kafka consumer application");
}
finally
{
    Log.Debug("[ORG>Consumer-0] exiting");
}