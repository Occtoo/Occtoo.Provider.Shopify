using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Occtoo.Provider.Shopify
{
    public static class ProductsFullImport
    {
        [FunctionName("ProductsFullImport")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("%product-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            que.Add("limit=3");
            return new OkObjectResult("Products full export started.");
        }
    }
}
