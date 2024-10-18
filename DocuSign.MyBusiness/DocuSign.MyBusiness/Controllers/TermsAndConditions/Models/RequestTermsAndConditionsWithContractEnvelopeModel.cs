using DocuSign.MyBusiness.Controllers.Common.Models;

namespace DocuSign.MyBusiness.Controllers.TermsAndConditions.Model
{
    public class RequestTermsAndConditionsWithContractEnvelopeModel
    {
        public SignerInfoModel SignerInfo { get; set; }
        public CarbonCopyInfoModel CarbonCopyInfo { get; set; }
    }
}
