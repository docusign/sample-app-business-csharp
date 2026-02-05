using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class RequestAccountAuthorizeModel : IValidatableObject
    {
        public string BasePath { get; set; }
        public string RedirectUrl { get; set; }
        public ConsentType ConsentType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(typeof(ConsentType), ConsentType))
            {
                yield return new ValidationResult("Invalid consent type.", new[] { nameof(ConsentType) });
            }

            if (string.IsNullOrWhiteSpace(BasePath))
            {
                yield return new ValidationResult("BasePath is required.", new[] { nameof(BasePath) });
            }
            else if (!IsValidHttpUrl(BasePath))
            {
                yield return new ValidationResult("BasePath must be a valid absolute URL.", new[] { nameof(BasePath) });
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
