using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;

namespace DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces
{
    public interface ICustomQuoteEnvelopeService
    {
        CreateEnvelopeResponse CreateCustomQuoteEnvelop(string accountId, SignerInfo signerInfo);
    }
}