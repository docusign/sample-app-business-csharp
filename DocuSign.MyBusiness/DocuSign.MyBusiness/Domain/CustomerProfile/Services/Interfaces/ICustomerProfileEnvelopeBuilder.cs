using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces
{
    public interface ICustomerProfileEnvelopeBuilder
    {
        EnvelopeDefinition BuildUserProfileUpdateEnvelope(string eventNotificationUrl, string clientUserId, string serverTemplateId, CustomerProfileSignerInfo signerInfo, AccountIdentityVerificationWorkflow identityVerificationWorkflow);
    }
}