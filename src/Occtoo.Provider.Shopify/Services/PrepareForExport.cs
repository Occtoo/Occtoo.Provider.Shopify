using Occtoo.Onboarding.Sdk.Models;
using Occtoo.Provider.Shopify.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify.Services
{
    public interface IPrepareForExport
    {
        Task<(List<DynamicEntity>, List<DynamicEntity>)> GetDynamicEntitiesAsync(List<CleanProductModel> model, bool mediaImport);
        Task<List<DynamicEntity>> GetDynamicEntitiesAsync(List<CleanLocationModel> model);
        Task<List<DynamicEntity>> GetDynamicEntitiesAsync(List<CleanInventoryModel> model);
    }

    public class PrepareForExport : IPrepareForExport
    {
        private readonly IOcctooMediaService _mediaService;
        private readonly AppSettings _appSettings;

        public PrepareForExport(IOcctooMediaService mediaService, AppSettings appSettings)
        {
            _mediaService = mediaService;
            _appSettings = appSettings;
        }

        private readonly List<string> knownImageExtensions = new()
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".tiff",
            ".psd",
            ".pdf",
            ".eps",
            ".ai",
            ".indd",
            ".raw",
            ".gif"
        };


        public async Task<(List<DynamicEntity>, List<DynamicEntity>)> GetDynamicEntitiesAsync(List<CleanProductModel> items, bool mediaImport)
        {
            return await Task.Run(async () =>
            {
                List<DynamicEntity> dynamicEntities = new();
                List<DynamicEntity> mediaDynamicEntities = new();

                foreach (var model in items)
                {
                    List<DynamicEntity> variantDynamicEntities = new();
                    DynamicEntity dynamicEntity = new()
                    {
                        Key = model.Id
                    };

                    foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                    {
                        if (propertyInfo.PropertyType != typeof(List<Media>))
                        {
                            string propertyValue = propertyInfo.GetValue(model, null)?.ToString() ?? null;
                            if (!string.IsNullOrEmpty(propertyValue))
                            {
                                if (!string.IsNullOrEmpty(propertyValue))
                                {
                                    // if one of our properties is media upload it to media service
                                    // and save it to the media source... You have to specify media props in app settings. 
                                    if (_appSettings.ListOfProductMediaObjects.Contains(propertyInfo.Name))
                                    {
                                        string fileName = Path.GetFileName(propertyValue).Split('?')[0];
                                        string fileExtension = Path.GetExtension(fileName).ToLower();

                                        var media = new Media { Filename = fileName, Src = propertyValue, Mainimage = propertyInfo.Name };
                                        var uploadedMedia = await _mediaService.GetMedia(media, fileName, CreateIdentifier("url", fileName));

                                        // can be expanded with more properties to have more media urls with reduced filesize upon download
                                        bool isSizedReduced = string.Equals(propertyInfo.Name, "Thumbnail");
                                        string importPolicy = isSizedReduced ? "?impolicy=small" : string.Empty;
                                        DynamicProperty dynProperty = new()
                                        {
                                            Id = propertyInfo.Name,
                                            Value = uploadedMedia.Url + importPolicy
                                        };
                                        dynamicEntity.Properties.Add(dynProperty);

                                        var dynamicEntityMedia = new DynamicEntity();
                                        dynamicEntityMedia.Key = media.Id;
                                        media.Url = uploadedMedia.Url + importPolicy;
                                        foreach (PropertyInfo mediaPropertyInfo in media.GetType().GetProperties())
                                        {
                                            dynamicEntityMedia.Properties.Add(
                                                new DynamicProperty
                                                {
                                                    Id = mediaPropertyInfo.Name,
                                                    Value = mediaPropertyInfo.GetValue(media, null)?.ToString()
                                                });
                                        }
                                        if (!variantDynamicEntities.Any(x => x.Key == dynamicEntityMedia.Key) && !isSizedReduced)
                                            variantDynamicEntities.Add(dynamicEntityMedia);
                                    }
                                    else
                                    {
                                        DynamicProperty dynProperty = new()
                                        {
                                            Id = propertyInfo.Name,
                                            Value = propertyValue
                                        };
                                        dynamicEntity.Properties.Add(dynProperty);
                                    }
                                }
                            }
                        }
                        else if (mediaImport == true)
                        {
                            List<Media> medias = (List<Media>)propertyInfo.GetValue(model, null);
                            foreach (var media in medias)
                            {
                                string fileName = Path.GetFileName(media.Src).Split('?')[0];
                                string fileExtension = Path.GetExtension(fileName).ToLower();

                                if (knownImageExtensions.Contains(fileExtension))
                                {
                                    if (!variantDynamicEntities.Any(x => x.Properties.Any(d => d.Value == media.ShopifyMediaId)))
                                    {
                                        var uploadedMedia = await _mediaService.GetMedia(media, fileName, CreateIdentifier(media.ShopifyMediaId, "url" + fileName));

                                        if (!string.IsNullOrEmpty(uploadedMedia.Id) && !string.IsNullOrEmpty(uploadedMedia.Url))
                                        {
                                            var dynamicEntityMedia = new DynamicEntity();
                                            dynamicEntityMedia.Key = media.Id;
                                            foreach (PropertyInfo mediaPropertyInfo in media.GetType().GetProperties())
                                            {
                                                dynamicEntityMedia.Properties.Add(
                                                    new DynamicProperty
                                                    {
                                                        Id = mediaPropertyInfo.Name,
                                                        Value = mediaPropertyInfo.GetValue(media, null)?.ToString()
                                                    });
                                            }
                                            if (!variantDynamicEntities.Any(x => x.Key == dynamicEntityMedia.Key))
                                                variantDynamicEntities.Add(dynamicEntityMedia);
                                        }
                                    }
                                }
                            }

                            dynamicEntity.Properties.Add(new DynamicProperty { Id = "media", Value = string.Join("|", variantDynamicEntities.Select(x => x.Key)) });
                            mediaDynamicEntities.AddRange(variantDynamicEntities);
                        }
                    }
                    dynamicEntities.Add(dynamicEntity);
                }
                return (dynamicEntities, mediaDynamicEntities.DistinctBy(x => x.Key).ToList());
            });
        }
        public async Task<List<DynamicEntity>> GetDynamicEntitiesAsync(List<CleanLocationModel> items)
        {
            return await Task.Run(() =>
            {
                List<DynamicEntity> dynamicEntities = new();
                foreach (var model in items)
                {
                    DynamicEntity dynamicEntity = new()
                    {
                        Key = model.LocationId
                    };

                    foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                    {
                        string propertyValue = propertyInfo.GetValue(model, null)?.ToString() ?? null;

                        if (!string.IsNullOrEmpty(propertyValue))
                        {
                            if (!string.IsNullOrEmpty(propertyValue))
                            {
                                DynamicProperty dynProperty = new()
                                {
                                    Id = propertyInfo.Name,
                                    Value = propertyValue
                                };
                                dynamicEntity.Properties.Add(dynProperty);
                            }
                        }
                    }
                    dynamicEntities.Add(dynamicEntity);
                }
                return dynamicEntities;
            });
        }
        public async Task<List<DynamicEntity>> GetDynamicEntitiesAsync(List<CleanInventoryModel> items)
        {
            return await Task.Run(() =>
            {
                List<DynamicEntity> dynamicEntities = new();
                foreach (var model in items)
                {
                    DynamicEntity dynamicEntity = new()
                    {
                        Key = model.InventoryItemId + "_" + model.LocationId
                    };

                    foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                    {
                        string propertyValue = propertyInfo.GetValue(model, null)?.ToString() ?? null;

                        if (!string.IsNullOrEmpty(propertyValue))
                        {
                            if (!string.IsNullOrEmpty(propertyValue))
                            {
                                DynamicProperty dynProperty = new()
                                {
                                    Id = propertyInfo.Name,
                                    Value = propertyValue
                                };
                                dynamicEntity.Properties.Add(dynProperty);
                            }
                        }
                    }
                    dynamicEntities.Add(dynamicEntity);
                }
                return dynamicEntities;
            });
        }

        private static string CreateIdentifier(string prefix, string filename) => prefix + "_" + filename;
    }
}