using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Occtoo.Provider.Shopify.Services
{
    public interface ITokenService
    {
        Task<string> GetProviderToken(string providerName, string id, string secret);
    }

    public class TokenService : ITokenService
    {
        private readonly string _partitionKey = "token";
        private readonly TableClient _tableClient;
        private readonly HttpClient _httpClient;

        public TokenService(IHttpClientFactory httpClientFactory, TableServiceClient tableServiceClient)
        {
            _httpClient = httpClientFactory.CreateClient("Default");
            _tableClient = tableServiceClient.GetTableClient("token");
            _tableClient.CreateIfNotExists();
        }

        public async Task<string> GetProviderToken(string providerName, string id, string secret)
        {
            var tokenResponse = await CheckForCachedTokenAsync(providerName);
            if (tokenResponse.HasValue)
            {
                var validTime = DateTime.UtcNow - tokenResponse.Value.Created;
                if (validTime.TotalMinutes < 50)
                {
                    return tokenResponse.Value.AccessToken;
                }
            }

            var tokenUrl = $"https://ingest.occtoo.com/dataProviders/tokens";
            var client = new
            {
                id,
                secret
            };
            var tokenInformation = await GetTokenInternal(tokenUrl, client);
            await AddTokenToCache(tokenInformation, providerName);
            return tokenInformation.AccessToken;
        }

        private async Task<TokenInformation> GetTokenInternal(string tokenUrl, object clientInformation)
        {
            var tokenResponse = await _httpClient.PostAsJsonAsync(tokenUrl, clientInformation);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenData = JsonConvert.DeserializeObject<TokenData>(await tokenResponse.Content.ReadAsStringAsync());

            return new TokenInformation
            {
                AccessToken = tokenData.result.accessToken
            };
        }

        private async Task AddTokenToCache(TokenInformation tokenInformation, string destinationId)
        {
            await _tableClient.DeleteEntityAsync(_partitionKey, destinationId);

            tokenInformation.Created = DateTime.UtcNow;
            tokenInformation.RowKey = destinationId;
            tokenInformation.PartitionKey = _partitionKey;

            await _tableClient.AddEntityAsync(tokenInformation);
        }

        private async Task<Azure.NullableResponse<TokenInformation>> CheckForCachedTokenAsync(string destinationId)
        {
            return await _tableClient.GetEntityIfExistsAsync<TokenInformation>(_partitionKey, destinationId);
        }
    }
    public class TokenInformation : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string AccessToken { get; set; }
        public DateTime Created { get; set; }
    }


    public class TokenData
    {
        public TokenInfo result { get; set; }
        public List<object> errors { get; set; }
        public string requestId { get; set; }
    }

    public class TokenInfo
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
        public string tokenType { get; set; }
        public object refreshToken { get; set; }
        public string scope { get; set; }
    }
}
