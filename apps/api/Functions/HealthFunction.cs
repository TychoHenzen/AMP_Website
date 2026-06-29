using Amp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Amp.Api.Functions;

/// <summary>
/// Liveness probe + a quick check that the shared Cosmos connection is configured.
/// Endpoint: GET /api/health
/// </summary>
public class HealthFunction
{
    private readonly CosmosDbConfig _config;

    public HealthFunction(CosmosDbConfig config) => _config = config;

    [Function("health")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
        => new OkObjectResult(new
        {
            status = "ok",
            cosmosConfigured = !string.IsNullOrWhiteSpace(_config.ConnectionString)
        });
}
