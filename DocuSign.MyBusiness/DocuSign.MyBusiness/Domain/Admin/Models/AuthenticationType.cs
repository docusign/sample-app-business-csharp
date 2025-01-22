using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocuSign.MyBusiness.Domain.Admin.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthenticationType
    {
        None = 0,
        UserAccount = 1,
        TestAccount = 2,
    }
}

