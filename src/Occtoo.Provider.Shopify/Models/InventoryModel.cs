using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Occtoo.Provider.Shopify.Models
{
    public class InventoryRoot
    {
        [JsonProperty("inventory_levels")]
        public List<InventoryModel> Inventories { get; set; }
    }
    public class InventoryModel
    {
        [JsonProperty("inventory_item_id")]
        public object InventoryItemId { get; set; }

        [JsonProperty("location_id")]
        public object LocationId { get; set; }

        [JsonProperty("available")]
        public int? Available { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphqlApiId { get; set; }
    }
}
