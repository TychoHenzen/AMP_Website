using Amp.Data;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Shared Cosmos config. Connection string comes from app settings (CosmosDb__ConnectionString) — never the repo.
var cosmosConfig = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbConfig>() ?? new CosmosDbConfig();
builder.Services.AddSingleton(cosmosConfig);
if (!string.IsNullOrWhiteSpace(cosmosConfig.ConnectionString))
{
    builder.Services.AddSingleton(_ => new CosmosClient(cosmosConfig.ConnectionString));
}

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
