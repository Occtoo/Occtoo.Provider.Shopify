namespace Occtoo.Provider.Shopify.Helpers
{
    public static class Helpers
    {
        public static string GetFullUrl(string url, string query)
        {
            return string.Concat(url, "?", query);
        }
    }
}
