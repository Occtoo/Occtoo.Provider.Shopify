using AutoMapper;
using Occtoo.Provider.Shopify.Models;
using System.Collections.Generic;
using System.Linq;

namespace Occtoo.Provider.Shopify.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<List<ProductModel>, IEnumerable<CleanProductModel>>().ConvertUsing<CleanProductModelConverter>();

            CreateMap<Location, CleanLocationModel>()
                .ForMember(dest => dest.LocationId, act => act.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address1, act => act.MapFrom(src => src.Address1))
                .ForMember(dest => dest.Address2, act => act.MapFrom(src => src.Address2))
                .ForMember(dest => dest.City, act => act.MapFrom(src => src.City))
                .ForMember(dest => dest.Zip, act => act.MapFrom(src => src.Zip))
                .ForMember(dest => dest.Province, act => act.MapFrom(src => src.Province))
                .ForMember(dest => dest.Country, act => act.MapFrom(src => src.Country))
                .ForMember(dest => dest.Phone, act => act.MapFrom(src => src.Phone))
                .ForMember(dest => dest.CreatedAt, act => act.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, act => act.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.CountryCode, act => act.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.CountryName, act => act.MapFrom(src => src.CountryName))
                .ForMember(dest => dest.ProvinceCode, act => act.MapFrom(src => src.ProvinceCode))
                .ForMember(dest => dest.Legacy, act => act.MapFrom(src => src.Legacy))
                .ForMember(dest => dest.Active, act => act.MapFrom(src => src.Active));

            CreateMap<InventoryModel, CleanInventoryModel>()
                .ForMember(dest => dest.InventoryItemId, act => act.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.LocationId, act => act.MapFrom(src => src.LocationId))
                .ForMember(dest => dest.Available, act => act.MapFrom(src => src.Available))
                .ForMember(dest => dest.UpdatedAt, act => act.MapFrom(src => src.UpdatedAt));
        }
    }

    class CleanProductModelConverter : ITypeConverter<List<ProductModel>, IEnumerable<CleanProductModel>>
    {
        public IEnumerable<CleanProductModel> Convert(List<ProductModel> source, IEnumerable<CleanProductModel> destination, ResolutionContext context)
        {
            foreach (var product in source)
            {

                foreach (var item in product.variants)
                {
                    CleanProductModel result = new()
                    {
                        Id = item.id.ToString(),
                        ItemTitle = item.title,
                        Sku = item.sku,
                        Status = product.status,
                        Price = item.price,
                        ItemPosition = item.position.ToString(),
                        Option1 = item.option1,
                        Option2 = item.option2,
                        Option3 = item.option3,
                        ImageId = item.image_id.ToString(),
                        Grams = item.grams.ToString(),
                        Barcode = item.barcode,
                        VariantCreatedAt = item.created_at.ToString(),
                        UpdatedAt = product.updated_at.ToString(),
                        RequiresShipping = item.requires_shipping.ToString(),
                        Weight = item.weight.ToString(),
                        WeightUnit = item.weight_unit,
                        InventoryItemId = item.inventory_item_id.ToString(),
                        InventoryQuantity = item.inventory_quantity.ToString(),
                        PresentmentPrices = "",
                        ProductId = product.id.ToString(),
                        ProductTitle = product.title,
                        Description = product.body_html,
                        Vendor = product.vendor,
                        ProductType = product.product_type,
                        ProductCreatedAt = product.created_at.ToString(),
                        Handle = product.handle,
                        ProductPublishedAt = product.published_at.ToString(),
                        PublishedScope = product.published_scope,
                        Tags = product.tags,
                        Colors = GetOptions(product.options, "Color"),
                        Sizes = GetOptions(product.options, "Talla"),
                        Thumbnail = product.image?.src ?? product.images.OrderBy(x => x.position).FirstOrDefault()?.src ?? "",
                        Medias = product.images.Select(x => new Media
                        {
                            CreatedAt = x.created_at.ToString(),
                            Height = x.height.ToString(),
                            Id = "",
                            Src = x.src,
                            ShopifyMediaId = x.id.ToString(),
                            Position = x.position.ToString(),
                            UpdatedAt = x.updated_at.ToString(),
                            Width = x.width.ToString(),
                            Variantids = string.Join('|', x.variant_ids.Select(x => x.Value).ToList()),
                        }).ToList()
                    };

                    if (!string.IsNullOrEmpty(result.Id))
                        yield return result;
                }
            }
        }

        private string GetOptions(List<Option> options, string fieldName)
        {
            var values = options.FirstOrDefault(x => x.name == fieldName);
            if (values != null)
            {
                var returnValue = string.Join('|', values.values);
                return returnValue;
            }
            return string.Empty;
        }
    }
}
