using DocuSign.MyBusiness.Infrustructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DocuSign.MyBusiness.Controllers.Common.Models
{
    public class ErrorDetailsModel
    {
        public string GeneralErrorMessage { get; set; }
        public IDictionary<string, string> Errors { get; set; }

        public static ErrorDetailsModel CreateErrorDetailsForOneModelProperty<TType, TReturn>(string errorCode, TType model, Expression<Func<TType, TReturn>> property)
        {
            return new ErrorDetailsModel
            {
                GeneralErrorMessage = errorCode,
                Errors = new Dictionary<string, string> { { property.GetPropertyName().ToCamelCase(), errorCode } }
            };
        }

        public static ErrorDetailsModel CreateErrorDetailsForTwoModelProperties<TType, TReturn>(
            string errorCode,
            TType model,
            Expression<Func<TType, TReturn>> property1,
            Expression<Func<TType, TReturn>> property2)
        {
            return new ErrorDetailsModel
            {
                GeneralErrorMessage = errorCode,
                Errors = new Dictionary<string, string>
                {
                    { property1.GetPropertyName().ToCamelCase(), errorCode },
                    { property2.GetPropertyName().ToCamelCase(), errorCode }
                }
            };
        }

        public static ErrorDetailsModel CreateErrorDetailsForManyModelProperty<TType, TReturn>(
            string generalErrorCode,
            TType model,
            IEnumerable<(string propertyErrorCode, Expression<Func<TType, TReturn>> property)> propertyErrors)
        {
            return new ErrorDetailsModel
            {
                GeneralErrorMessage = generalErrorCode,
                Errors = propertyErrors.ToDictionary(
                    x => x.property.GetPropertyName().ToCamelCase(),
                    x => x.propertyErrorCode)
            };
        }

        public static ErrorDetailsModel CreateGeneralErrorDetails(string errorCode)
        {
            return new ErrorDetailsModel
            {
                GeneralErrorMessage = errorCode
            };
        }
    }
}
