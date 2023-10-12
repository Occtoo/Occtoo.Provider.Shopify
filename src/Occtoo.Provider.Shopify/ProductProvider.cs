using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Onboarding.Sdk;
using Occtoo.Provider.Shopify.Models;
using Occtoo.Provider.Shopify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify
{
    public class ProductProvider
    {
        private readonly IApiService<ProductRoot> _apiService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IPrepareForExport _export;
        private readonly ITokenService _tokenService;

        public ProductProvider(IApiService<ProductRoot> apiService, AppSettings appSettings, IMapper mapper, IPrepareForExport export, ITokenService tokenService)
        {
            _apiService = apiService;
            _appSettings = appSettings;
            _mapper = mapper;
            _export = export;
            _tokenService = tokenService;
        }

        [FunctionName(nameof(ProductProvider))]
        public async Task Run([QueueTrigger("product-query-items", Connection = "AzureWebJobsStorage")] string myQueueItem,
            [Queue("%product-query-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            try
            {
                log.LogInformation($"C# Product Queue trigger function processed: {myQueueItem}");
                var api = Helpers.Helpers.GetFullUrl(_appSettings.ProductApiCombined, myQueueItem);
                var model = await _apiService.GetAll(api);

                var cleanModel = _mapper.Map<IEnumerable<CleanProductModel>>(model.Results.Products);

                var exportEntities = await _export.GetDynamicEntitiesAsync(cleanModel.ToList(), true);

                var onboardingServliceClient = new OnboardingServiceClient(_appSettings.DataProviderClientId, _appSettings.DataProviderClientSecret);
                var token = await _tokenService.GetProviderToken("StockPriceOnboarder", Environment.GetEnvironmentVariable("DataProviderId"), Environment.GetEnvironmentVariable("DataProviderSecret"));
                var response = await onboardingServliceClient.StartEntityImportAsync(_appSettings.OcctooSourceProducts, exportEntities.Item1, token);
                var responseMedia = await onboardingServliceClient.StartEntityImportAsync(_appSettings.OcctooSourceMedia, exportEntities.Item2, token);

                if (!string.IsNullOrEmpty(model.NewQueryString))
                    que.Add("limit=3&" + model.NewQueryString);
            }
            catch (Exception ex)
            {
                log.LogError("Error on product import trigger for query " + myQueueItem, ex.Message + ex.StackTrace + ex.ToString());
                que.Add(myQueueItem);
            }
        }
    }
}
