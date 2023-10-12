using System.Collections.Generic;

namespace Occtoo.Provider.Shopify.Models
{
    public class AppSettings
    {
        public string AzureWebJobsStorage { get; set; }

        // email lists for errors
        public List<string> ListOfProductMediaObjects { get; set; }
        public string ApiUrl { get; set; }
        public string ApiDateVersion { get; set; }
        public string ProductsUrl { get; set; }
        public string LocationsUrl { get; set; }
        public string InventoryUrl { get; set; }
        public string DataProviderClientId { get; set; }
        public string DataProviderClientSecret { get; set; }
        public string ExecutionTime { get; set; }
        public string ShopifyAccessToken { get; set; }

        // api urls without query strings.
        public string ProductApiCombined
        {
            get { return ApiUrl.Replace("[DATE_REPLACE]", ApiDateVersion).Replace("[FILE_REPLACE]", ProductsUrl); }
        }

        public string LocationsApiCombined
        {
            get { return ApiUrl.Replace("[DATE_REPLACE]", ApiDateVersion).Replace("[FILE_REPLACE]", LocationsUrl); }
        }

        public string InventoryApiCombined
        {
            get { return ApiUrl.Replace("[DATE_REPLACE]", ApiDateVersion).Replace("[FILE_REPLACE]", InventoryUrl); }
        }

        // names of sources inside studio
        public string OcctooSourceProducts { get; set; }
        public string OcctooSourceInventory { get; set; }
        public string OcctooSourceLocations { get; set; }
        public string OcctooSourceMedia { get; set; }
    }
}
