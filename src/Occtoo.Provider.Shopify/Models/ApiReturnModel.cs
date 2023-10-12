namespace Occtoo.Provider.Shopify.Models
{
    public class ApiReturnModel<T>
    {
        public string NewQueryString { get; set; }
        public T Results { get; set; }
    }
}
