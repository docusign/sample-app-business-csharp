using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Models;

namespace DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces
{
    public interface ITermsAndConditionsEnvelopeBuilder
    {
        EnvelopeDefinition BuildTermsAndConditionsWithContractEnvelope(string eventNotificationUrl, string contractTemplateId, string termsAndConditionsTemplateId, SignerInfo signerInfo, CarbonCopyInfo carbonCopyInfo);
    }
}