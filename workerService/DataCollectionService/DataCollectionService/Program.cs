using DataCollectionService;
using DataCollectionService.Services.BackgroundServices;
using DataCollectionService.Services;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Hosting.WindowsServices;

[assembly: InternalsVisibleTo("DataCollectionService.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

var _logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
_logger.Debug("init main");

try
{

    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "CloudlabDataCollectService";
        })
        .ConfigureServices(services =>
        {
            ServiceProviderExtension.AddIGSMConnectionSingletonService(services);
            ServiceProviderExtension.AddIRabbitMQPersistConnSingletonService(services);
            ServiceProviderExtension.AddRabbitPublisherSingletonService(services);
            ServiceProviderExtension.AddIRabbitMessageGenSingletonService(services);
            ServiceProviderExtension.AddIIndicationsReadingSessionSingletonService(services);
            ServiceProviderExtension.AddIIndicationsReaderSingletonService(services);
            ServiceProviderExtension.AddRabbitMQConsumerSingletonService(services);

            ServiceProviderExtension.AddRabbitMQConsumerBackHostedService(services);
        })
        .ConfigureLogging(c =>
        {
            c.ClearProviders();
        })
        .UseNLog()
        .Build();


    await host.RunAsync();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    _logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
