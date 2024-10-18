using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Models;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Domain.EmploymentContract.Services.Interfaces
{
    public interface IEmploymentContractEnvelopeService
    {
        CreateEnvelopeResponse CreateEmploymentContractEnvelop(EnvelopeAction envelopAction, string templateId, string accountId, string redirectUrl, SignerInfo signerInfo, SignatureInfo signatureInfo);
    }
}