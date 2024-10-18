using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Models;

namespace DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces
{
    public interface ITermsAndConditionsEnvelopeService
    {
        CreateEnvelopeResponse CreateTermsAndConditionsWithContractEnvelop(string accountId, SignerInfo signerInfo, CarbonCopyInfo carbonCopyInfo);
    }
}