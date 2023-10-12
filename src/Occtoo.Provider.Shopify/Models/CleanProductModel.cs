using System.Collections.Generic;

namespace Occtoo.Provider.Shopify.Models
{
    public class CleanProductModel
    {
        public string Id { get; set; }
        public string ItemTitle { get; set; }
        public string Sku { get; set; }
        public string Price { get; set; }
        public string ItemPosition { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string ImageId { get; set; }
        public string Grams { get; set; }
        public string Barcode { get; set; }
        public string VariantCreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string RequiresShipping { get; set; }
        public string Thumbnail { get; set; }
        public string Weight { get; set; }
        public string WeightUnit { get; set; }
        public string InventoryItemId { get; set; }
        public string InventoryQuantity { get; set; }
        public string PresentmentPrices { get; set; }
        public string ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public string ProductType { get; set; }
        public string ProductCreatedAt { get; set; }
        public string Handle { get; set; }
        public string ProductPublishedAt { get; set; }
        public string Status { get; set; }
        public string PublishedScope { get; set; }
        public string Tags { get; set; }
        public List<Media> Medias { get; set; }
        public string Colors { get; set; }
        public string Sizes { get; set; }
    }
}
