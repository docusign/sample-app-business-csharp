using Newtonsoft.Json;
using System;

namespace DocuSign.MyBusiness.Infrustructure.Model
{
    [Serializable]
    public class ApiErrorDetails
    {
        public const string InvalidUserId = "invalid_grant";
        public const string InvalidAccountId = "invalid_account";
        public const string InvalidBasePath = "invalid_base_path";
        public const string InvalidBaseUri = "invalid_base_uri";
        public const string InvalidUserName = "invalid_user";
        public const string InvalidPassword = "invalid_password";
        public const string InvalidAuthentication = "invalid_authentication";


        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        public static ApiErrorDetails CreateInvalidAccountIdError()
        {
            return new ApiErrorDetails
            {
                Error = InvalidAccountId
            };
        }

        public static ApiErrorDetails CreateInvalidBaseUriError()
        {
            return new ApiErrorDetails
            {
                Error = InvalidBaseUri
            };
        }

        public static ApiErrorDetails CreateInvalidBasePathError()
        {
            return new ApiErrorDetails
            {
                Error = InvalidBasePath
            };
        }
    }
}
