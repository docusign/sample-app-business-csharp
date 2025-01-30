using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocuSign.MyBusiness.Domain.Admin.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthenticationType
    {
        UserAccount = 1,
        TestAccount = 2,
        FreeTrial = 3,
    }
}

