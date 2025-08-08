using DatadogWebhookFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Add Application Insights telemetry
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register custom services
        services.AddScoped<IWebhookProcessingService, WebhookProcessingService>();

        // Add logging
        services.AddLogging();
    })
    .Build();

host.Run();
