#region

using DuurzaamDigitaal.Components;
using DuurzaamDigitaal.Data;
using DuurzaamDigitaal.Data.Repositories;
using Microsoft.Azure.Cosmos;

#endregion

namespace DuurzaamDigitaal;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Cosmos DB
        var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb").Get<CosmosDbConfig>()
                             ?? throw new InvalidOperationException("Cosmos DB configuration is missing.");

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add Cosmos DB client
        builder.Services.AddSingleton(cosmosDbConfig);
        builder.Services.AddSingleton(sp =>
        {
            var cosmosClient = new CosmosClient(cosmosDbConfig.ConnectionString);
            var database = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbConfig.DatabaseId).GetAwaiter()
                .GetResult();

            const string partitionKeyPath = "/partitionKey";
            // Create containers if they don't exist
            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.MessagesContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.AppointmentsContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.TimeSlotsContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.RefurbishedDevicesContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.InvoicesContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.PaymentsContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(cosmosDbConfig.AdminUsersContainerId, partitionKeyPath)
            ).GetAwaiter().GetResult();

            return cosmosClient;
        });

        // Register repositories
        builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        builder.Services.AddScoped<IRefurbishedDeviceRepository, RefurbishedDeviceRepository>();
        builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

        var app = builder.Build();


        if (app.Environment.IsDevelopment())
        {
            app.Urls.Add("http://0.0.0.0:5258");
            app.Urls.Add("https://0.0.0.0:7038");
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}