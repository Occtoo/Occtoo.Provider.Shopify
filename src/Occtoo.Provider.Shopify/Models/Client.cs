using Newtonsoft.Json;

namespace Occtoo.Provider.Shopify.Models
{
    internal class Client
    {
        public Client(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }
    }
}
