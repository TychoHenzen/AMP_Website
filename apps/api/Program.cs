using Amp.Api.Nido;
using Amp.Data;
using Amp.Data.Nido;
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

// Nido booking data lives in its own Cosmos database (defaults: db "nido", container "appointments").
var nidoConfig = builder.Configuration.GetSection("Nido").Get<NidoConfig>() ?? new NidoConfig();
builder.Services.AddSingleton(nidoConfig);

// Azure Communication Services email (booking notifications). No-ops when unconfigured.
var acsConfig = builder.Configuration.GetSection("Acs").Get<AcsConfig>() ?? new AcsConfig();
builder.Services.AddSingleton(acsConfig);
builder.Services.AddSingleton<BookingEmailService>();

if (!string.IsNullOrWhiteSpace(cosmosConfig.ConnectionString))
{
    builder.Services.AddSingleton(_ => new CosmosClient(cosmosConfig.ConnectionString));
    builder.Services.AddSingleton<INidoAppointmentRepository, NidoAppointmentRepository>();
}

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
