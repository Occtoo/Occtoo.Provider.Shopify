using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Occtoo.Provider.Shopify.Models
{
    public class Image
    {
        [JsonProperty("id")]
        public object id { get; set; }

        [JsonProperty("product_id")]
        public object product_id { get; set; }

        [JsonProperty("position")]
        public int? position { get; set; }

        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }

        [JsonProperty("alt")]
        public object alt { get; set; }

        [JsonProperty("width")]
        public int? width { get; set; }

        [JsonProperty("height")]
        public int? height { get; set; }

        [JsonProperty("src")]
        public string src { get; set; }

        [JsonProperty("variant_ids")]
        public List<long?> variant_ids { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string admin_graphql_api_id { get; set; }
    }

    public class Image2
    {
        [JsonProperty("id")]
        public long? id { get; set; }

        [JsonProperty("product_id")]
        public long? product_id { get; set; }

        [JsonProperty("position")]
        public int? position { get; set; }

        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }

        [JsonProperty("alt")]
        public object alt { get; set; }

        [JsonProperty("width")]
        public int? width { get; set; }

        [JsonProperty("height")]
        public int? height { get; set; }

        [JsonProperty("src")]
        public string src { get; set; }

        [JsonProperty("variant_ids")]
        public List<long?> variant_ids { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string admin_graphql_api_id { get; set; }
    }

    public class Option
    {
        [JsonProperty("id")]
        public object id { get; set; }

        [JsonProperty("product_id")]
        public object product_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("position")]
        public int? position { get; set; }

        [JsonProperty("values")]
        public List<string> values { get; set; }
    }

    public class ProductRoot
    {
        public List<ProductModel> Products { get; set; }
    }

    public class ProductModel
    {
        [JsonProperty("id")]
        public long? id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("body_html")]
        public string body_html { get; set; }

        [JsonProperty("vendor")]
        public string vendor { get; set; }

        [JsonProperty("product_type")]
        public string product_type { get; set; }

        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }

        [JsonProperty("handle")]
        public string handle { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }

        [JsonProperty("published_at")]
        public DateTime? published_at { get; set; }

        [JsonProperty("template_suffix")]
        public string template_suffix { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("published_scope")]
        public string published_scope { get; set; }

        [JsonProperty("tags")]
        public string tags { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string admin_graphql_api_id { get; set; }

        [JsonProperty("variants")]
        public List<Variant> variants { get; set; }

        [JsonProperty("options")]
        public List<Option> options { get; set; }

        [JsonProperty("images")]
        public List<Image> images { get; set; }

        [JsonProperty("image")]
        public Image image { get; set; }
    }

    public class Root
    {
        [JsonProperty("products")]
        public List<ProductModel> products { get; set; }
    }

    public class Variant
    {
        [JsonProperty("id")]
        public long? id { get; set; }

        [JsonProperty("product_id")]
        public long? product_id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("price")]
        public string price { get; set; }

        [JsonProperty("sku")]
        public string sku { get; set; }

        [JsonProperty("position")]
        public int? position { get; set; }

        [JsonProperty("inventory_policy")]
        public string inventory_policy { get; set; }

        [JsonProperty("compare_at_price")]
        public string compare_at_price { get; set; }

        [JsonProperty("fulfillment_service")]
        public string fulfillment_service { get; set; }

        [JsonProperty("inventory_management")]
        public string inventory_management { get; set; }

        [JsonProperty("option1")]
        public string option1 { get; set; }

        [JsonProperty("option2")]
        public string option2 { get; set; }

        [JsonProperty("option3")]
        public string option3 { get; set; }

        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }

        [JsonProperty("taxable")]
        public bool? taxable { get; set; }

        [JsonProperty("barcode")]
        public string barcode { get; set; }

        [JsonProperty("grams")]
        public int? grams { get; set; }

        [JsonProperty("image_id")]
        public long? image_id { get; set; }

        [JsonProperty("weight")]
        public double? weight { get; set; }

        [JsonProperty("weight_unit")]
        public string weight_unit { get; set; }

        [JsonProperty("inventory_item_id")]
        public long? inventory_item_id { get; set; }

        [JsonProperty("inventory_quantity")]
        public int? inventory_quantity { get; set; }

        [JsonProperty("old_inventory_quantity")]
        public int? old_inventory_quantity { get; set; }

        [JsonProperty("tax_code")]
        public string tax_code { get; set; }

        [JsonProperty("requires_shipping")]
        public bool? requires_shipping { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string admin_graphql_api_id { get; set; }
    }

}
