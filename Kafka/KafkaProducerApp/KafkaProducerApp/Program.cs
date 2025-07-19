using KafkaProducerApp;
using KafkaProducerApp.DataModels;
using KafkaProducerApp.Services;
using Serilog;

try
{
    IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .UseSerilog((context, services, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration)
              .Enrich.FromLogContext();
    })
    .ConfigureServices((context, services) =>
    {
        /*services.AddSingleton<PartitionConfigProvider>();
        services.AddSingleton<IPartitionPolicy, HashPartitionPolicy>();*/
        services.Configure<KafkaSettings>(context.Configuration.GetSection("KafkaSettings"));
        services.AddTransient<KafkaProducerService>();
        services.AddHostedService<Worker>();

    })
    .Build();

    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}