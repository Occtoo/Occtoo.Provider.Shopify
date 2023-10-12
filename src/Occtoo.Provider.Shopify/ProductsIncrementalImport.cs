using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify
{
    public class ProductIncrementalImport
    {
        [FunctionName(nameof(ProductIncrementalImport))]
#if DEBUG
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("%product-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que)
#else
        public void Run([TimerTrigger("%CRON_TIMER_SETTING_STRING_PRODUCT%")] TimerInfo myTimer,
        [Queue("%product-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que)
#endif
        {
            var recordExecutionDate = DateTime.UtcNow.AddDays(-3.2d).ToString("yyyy-MM-ddTHH:mm:ss-mm:ss-00:00");
            var query = $"limit=3&updated_at_min={recordExecutionDate}";
            que.Add(query);
        }
    }
}