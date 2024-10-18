using System.Text.Json;

namespace DocuSign.MyBusiness.Infrustructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : JsonNamingPolicy.CamelCase.ConvertName(value);
        }
    }
}
