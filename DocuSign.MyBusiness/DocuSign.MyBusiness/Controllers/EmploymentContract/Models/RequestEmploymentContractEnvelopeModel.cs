using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Models;

namespace DocuSign.MyBusiness.Controllers.EmploymentContract.Model
{
    public class RequestEmploymentContractEnvelopeModel
    {
        public EnvelopeAction EnvelopeAction { get; set; }
        public string Template { get; set; }
        public string RedirectUrl { get; set; }
        public SignerInfoModel SignerInfo { get; set; }
        public SignatureInfoModel SignatureInfo { get; set; }
    }
}
