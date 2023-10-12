using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Onboarding.Sdk;
using Occtoo.Provider.Shopify.Models;
using Occtoo.Provider.Shopify.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify
{
    public class InventoryProvider
    {
        private readonly IApiService<InventoryRoot> _apiService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IPrepareForExport _export;
        private readonly ITokenService _tokenService;

        public InventoryProvider(IApiService<InventoryRoot> apiService, AppSettings appSettings, IMapper mapper, IPrepareForExport export, ITokenService tokenService)
        {
            _apiService = apiService;
            _appSettings = appSettings;
            _mapper = mapper;
            _export = export;
            _tokenService = tokenService;
        }

        [FunctionName(nameof(InventoryProvider))]
        public async Task Run([QueueTrigger("inventory-query-items", Connection = "AzureWebJobsStorage")] string myQueueItem,
            [Queue("%inventory-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            try
            {
                log.LogInformation($"C# Inventory Queue trigger function processed: {myQueueItem}");
                var api = Helpers.Helpers.GetFullUrl(_appSettings.InventoryApiCombined, myQueueItem);
                var model = await _apiService.GetAll(api);

                var cleanModel = _mapper.Map<List<CleanInventoryModel>>(model.Results.Inventories);

                var exportEntities = await _export.GetDynamicEntitiesAsync(cleanModel);

                var token = await _tokenService.GetProviderToken("StockPriceOnboarder", Environment.GetEnvironmentVariable("DataProviderId"), Environment.GetEnvironmentVariable("DataProviderSecret"));
                var onboardingServliceClient = new OnboardingServiceClient(_appSettings.DataProviderClientId, _appSettings.DataProviderClientSecret);
                var response = await onboardingServliceClient.StartEntityImportAsync(_appSettings.OcctooSourceInventory, exportEntities, token);

                if (!string.IsNullOrEmpty(model.NewQueryString))
                    que.Add("limit=250&" + model.NewQueryString);
            }
            catch (Exception ex)
            {
                log.LogError("Exception happened on inventory import " + myQueueItem, ex.Message + ex.StackTrace);
            }
        }
    }
}
