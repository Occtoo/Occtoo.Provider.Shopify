using Azure.Storage.Blobs;
using Occtoo.Provider.Shopify.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify.Services
{
    public interface IBlobService
    {
        Task UploadExecutionTime(MemoryStream blob, string filename);
        Task<DateTime> GetLastExecutionTime(string fileName);
    }
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AppSettings _appSettings;

        public BlobService(AppSettings appSettings)
        {
            _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), new BlobClientOptions { });
            _appSettings = appSettings;
        }

        public async Task<DateTime> GetLastExecutionTime(string filename)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_appSettings.ExecutionTime);
            await blobContainerClient.CreateIfNotExistsAsync();
            var blob = blobContainerClient.GetBlobClient(filename);

            if (await blob.ExistsAsync())
            {
                var blobContents = await blob.DownloadContentAsync();
                if (DateTime.TryParse(blobContents.Value.Content.ToString(), out DateTime datetime))
                    return datetime;
            }
            return new DateTime(2018, 12, 31);
        }

        public async Task UploadExecutionTime(MemoryStream blob, string filename)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("ExecutionTime"));
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(filename);
            if (!await blobClient.ExistsAsync())
            {
                await blobContainerClient.UploadBlobAsync("ExecutionTime", blob);
            }
            else
            {
                await blobClient.DeleteAsync();
                await UploadExecutionTime(blob, filename);
            }
        }
    }
}
