using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Models;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Domain.EmploymentContract.Services.Interfaces
{
    public interface IEmploymentContractEnvelopeBuilder
    {
        EnvelopeDefinition BuildEmploymentContractEnvelope(string eventNotificationUrl, EnvelopeAction envelopAction, string serverTemplateId, SignerInfo signerInfo, SignatureInfo signatureInfo);
    }
}