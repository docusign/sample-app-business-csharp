using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocuSign.MyBusiness.Domain.Admin.Models;

namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class RequestAccountConnectModel : IValidatableObject
    {
        public AuthenticationType AuthenticationType { get; set; }
        public string BasePath { get; set; }
        public string BaseUri { get; set; }
        public string AccountId { get; set; }
        public string UserId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(typeof(AuthenticationType), AuthenticationType))
            {
                yield return new ValidationResult("Invalid authentication type.", new[] { nameof(AuthenticationType) });
                yield break;
            }

            if (AuthenticationType == AuthenticationType.UserAccount)
            {
                if (string.IsNullOrWhiteSpace(BasePath))
                {
                    yield return new ValidationResult("BasePath is required.", new[] { nameof(BasePath) });
                }
                else if (!IsValidHttpUrl(BasePath))
                {
                    yield return new ValidationResult("BasePath must be a valid absolute URL.", new[] { nameof(BasePath) });
                }

                if (string.IsNullOrWhiteSpace(BaseUri))
                {
                    yield return new ValidationResult("BaseUri is required.", new[] { nameof(BaseUri) });
                }
                else if (!IsValidHttpUrl(BaseUri))
                {
                    yield return new ValidationResult("BaseUri must be a valid absolute URL.", new[] { nameof(BaseUri) });
                }

                if (string.IsNullOrWhiteSpace(AccountId))
                {
                    yield return new ValidationResult("AccountId is required.", new[] { nameof(AccountId) });
                }
                else if (!Guid.TryParse(AccountId, out _))
                {
                    yield return new ValidationResult("AccountId must be a valid GUID.", new[] { nameof(AccountId) });
                }

                if (string.IsNullOrWhiteSpace(UserId))
                {
                    yield return new ValidationResult("UserId is required.", new[] { nameof(UserId) });
                }
                else if (!Guid.TryParse(UserId, out _))
                {
                    yield return new ValidationResult("UserId must be a valid GUID.", new[] { nameof(UserId) });
                }
            }
        }

        private static bool IsValidHttpUrl(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp;
        }
    }
}

