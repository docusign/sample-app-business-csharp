using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.Common.Models;

namespace DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces
{
    public interface ICustomQuoteEnvelopeBuilder
    {
        EnvelopeDefinition BuildCustomQuoteEnvelope(string eventNotificationUrl, SignerInfo signerInfo);
    }
}