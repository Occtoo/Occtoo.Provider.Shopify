using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Occtoo.Provider.Shopify
{
    public static class InventoryFullImport
    {
        [FunctionName(nameof(InventoryFullImport))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("%inventory-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            que.Add("location_ids=16202268721&limit=250");
            return new OkObjectResult("Full import started");
        }
    }
}
