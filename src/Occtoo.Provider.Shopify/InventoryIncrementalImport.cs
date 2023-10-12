using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify
{
    public class InventoryIncrementalImport
    {

        [FunctionName(nameof(InventoryIncrementalImport))]
#if DEBUG
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("%inventory-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que)
#else

        public void Run([TimerTrigger("%CRON_TIMER_SETTING_STRING_INVENTORY%")] TimerInfo myTimer,
            [Queue("%inventory-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que)
#endif
        {
            var recordExecutionDate = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ss-mm:ss-00:00");
            var query = $"location_ids=16202268721&limit=250&updated_at_min={recordExecutionDate}";
            que.Add(query);
        }
    }
}
