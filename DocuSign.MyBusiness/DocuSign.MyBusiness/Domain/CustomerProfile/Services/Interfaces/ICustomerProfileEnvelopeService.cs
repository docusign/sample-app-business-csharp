using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces
{
    public interface ICustomerProfileEnvelopeService
    {
        CreateEnvelopeResponse CreateUpdateProfileEnvelope(string accountId, string redirectUrl, CustomerProfileSignerInfo signerInfo);
    }
}