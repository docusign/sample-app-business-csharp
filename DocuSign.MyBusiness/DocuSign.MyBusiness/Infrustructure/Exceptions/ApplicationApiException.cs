using DocuSign.eSign.Client;
using DocuSign.MyBusiness.Infrustructure.Model;
using Newtonsoft.Json;
using System;
using System.Net;

namespace DocuSign.MyBusiness.Infrustructure.Exceptions
{
    public class ApplicationApiException : ApplicationException
    {
        public ApiErrorDetails Details { get; private set; }

        private ApplicationApiException(ApiErrorDetails details)
        {
            Details = details;
        }

        public ApplicationApiException(ApiException ex)
        {
            Details = ParseApiException(ex);
        }

        public static ApplicationApiException CreateInvalidAccountException()
        {
            return new ApplicationApiException(ApiErrorDetails.CreateInvalidAccountIdError());
        }

        public static Exception CreateInvalidBaseUriException()
        {
            return new ApplicationApiException(ApiErrorDetails.CreateInvalidBaseUriError());
        }

        private ApiErrorDetails ParseApiException(ApiException ex)
        {
            switch ((HttpStatusCode)ex.ErrorCode)
            {
                case HttpStatusCode.InternalServerError:
                    return ApiErrorDetails.CreateInvalidBaseUriError();
                case HttpStatusCode.BadRequest:
                    return JsonConvert.DeserializeObject<ApiErrorDetails>(ex.ErrorContent);
                default:
                    return new ApiErrorDetails();
            }
        }
    }
}
