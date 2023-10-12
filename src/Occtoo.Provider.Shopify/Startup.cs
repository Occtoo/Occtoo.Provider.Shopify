using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Occtoo.Provider.Shopify;
using Occtoo.Provider.Shopify.Mappers;
using Occtoo.Provider.Shopify.Models;
using Occtoo.Provider.Shopify.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Occtoo.Provider.Shopify
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logger => { logger.SetMinimumLevel(LogLevel.Trace); });
            builder.Services.AddHttpClient("Default")
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            ConfigureServices(builder.Services);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ParseSettingsFromEnvironmentVariables());
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IPrepareForExport, PrepareForExport>();
            services.AddTransient(typeof(IApiService<>), typeof(ApiService<>));
            services.AddSingleton<IOcctooMediaService, OcctooMediaService>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public AppSettings ParseSettingsFromEnvironmentVariables()
        {
            var settings = new AppSettings
            {
                AzureWebJobsStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                ApiUrl = Environment.GetEnvironmentVariable("ApiUrl"),
                ApiDateVersion = Environment.GetEnvironmentVariable("ApiDateVersion"),
                ProductsUrl = Environment.GetEnvironmentVariable("ProductsUrl"),
                LocationsUrl = Environment.GetEnvironmentVariable("LocationsUrl"),
                InventoryUrl = Environment.GetEnvironmentVariable("InventoryUrl"),
                OcctooSourceProducts = Environment.GetEnvironmentVariable("OcctooSourceProducts"),
                OcctooSourceInventory = Environment.GetEnvironmentVariable("OcctooSourceInventory"),
                ListOfProductMediaObjects = JsonSerializer.Deserialize<List<string>>(Environment.GetEnvironmentVariable("ListOfProductMediaObjects")),
                OcctooSourceLocations = Environment.GetEnvironmentVariable("OcctooSourceLocations"),
                OcctooSourceMedia = Environment.GetEnvironmentVariable("OcctooSourceMedia"),
                ShopifyAccessToken = Environment.GetEnvironmentVariable("ShopifyAccessToken"),
                DataProviderClientId = Environment.GetEnvironmentVariable("DataProviderClientId"),
                DataProviderClientSecret = Environment.GetEnvironmentVariable("DataProviderClientSecret")
            };
            return settings;
        }
    }
}
