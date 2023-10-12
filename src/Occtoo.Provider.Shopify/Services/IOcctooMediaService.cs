using Occtoo.Onboarding.Sdk;
using Occtoo.Onboarding.Sdk.Models;
using Occtoo.Provider.Shopify.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify.Services
{
    public interface IOcctooMediaService
    {
        Task<Media> GetMedia(Media media, string fileName, string uniqueIdentifier);
    }

    public class OcctooMediaService : IOcctooMediaService
    {
        private readonly AppSettings _appSettings;
        private readonly OnboardingServiceClient _serviceClient;

        public OcctooMediaService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _serviceClient = new OnboardingServiceClient(_appSettings.DataProviderClientId, _appSettings.DataProviderClientSecret);
        }

        public async Task<Media> GetMedia(Media media, string filename, string uniqueIdentifier)
        {
            if (!string.IsNullOrEmpty(media.Src))
            {
                var mediaObj = await UploadFileToOcctooMediaService(media.Src, filename, uniqueIdentifier);
                media.Url = mediaObj.Url;
                media.Id = mediaObj.Key;
                media.Filename = filename;
                media.Thumbnail = mediaObj.Url + "?impolicy=small";
                return media;
            }
            return media;
        }

        /// <summary>
        /// Returns the url from occtoo media service.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<EntityMedia> UploadFileToOcctooMediaService(string url, string filename, string uniqueId)
        {
            var apiResult = _serviceClient.GetFileFromUniqueId(uniqueId);
            if (apiResult.StatusCode != 200 || string.IsNullOrEmpty(apiResult.Result.Id))
            {
                var cancellationToken = new CancellationTokenSource(180000).Token; // 3 mins
                apiResult = await _serviceClient.UploadFromLinkAsync(new FileUploadFromLink(url, filename) { UniqueIdentifier = uniqueId }, null, cancellationToken);
                if (apiResult.StatusCode != 200)
                {
                    throw new Exception($"File {filename} on url: {url}: upload failed - check if media service is up and running.");
                }
            }

            return new EntityMedia
            {
                Key = apiResult.Result.Id,
                Url = apiResult.Result.PublicUrl,
                OriginalUrl = apiResult.Result.SourceUrl
            };
        }
    }
}
