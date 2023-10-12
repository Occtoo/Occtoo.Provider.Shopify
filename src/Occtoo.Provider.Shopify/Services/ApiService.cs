using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Occtoo.Provider.Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify.Services
{
    public interface IApiService<T>
    {
        Task<ApiReturnModel<T>> GetAll(string api);
    }

    public class ApiService<T> : IApiService<T>
    {
        private readonly IBlobService _blobService;
        private readonly AppSettings _appSettings;

        public ApiService(IBlobService blobService, AppSettings appSettings)
        {
            _blobService = blobService;
            _appSettings = appSettings;
        }
        public async Task<ApiReturnModel<T>> GetAll(string api)
        {
            ApiReturnModel<T> returnModel = new();

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", _appSettings.ShopifyAccessToken);
            var response = await httpClient.GetAsync(api);


            ITraceWriter traceWriter = new MemoryTraceWriter();
            try
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<T>(responseJson,
                    new JsonSerializerSettings { TraceWriter = traceWriter });
                returnModel.Results = model;

                response.Headers.TryGetValues("Link", out IEnumerable<string> linkValue);
                if (linkValue != null)
                    returnModel.NewQueryString = GetNewQuery(linkValue);
                return returnModel;
            }
            catch (Exception ex)
            {
                throw new Exception("api call: " + api + " - " + traceWriter.ToString() + ex.Message);
            }
        }

        public string GetNewQuery(IEnumerable<string> linkValue)
        {
            string newQuery = "";
            if (linkValue.Any())
            {
                var firstValue = linkValue.FirstOrDefault();
                var next = firstValue.Split(',');

                foreach (var parameter in next)
                {
                    if (parameter.Contains("rel=\"next\""))
                    {
                        var regex = new Regex(@"page_info=(?:(?!>).)*");
                        var match = regex.Match(parameter);
                        newQuery = match.Value;
                    }
                }
            }
            return newQuery;
        }
    }

    internal class Product
    {
        public string language { get; set; }
        public IReadOnlyCollection<ProductModel> results { get; set; }
    }
}
