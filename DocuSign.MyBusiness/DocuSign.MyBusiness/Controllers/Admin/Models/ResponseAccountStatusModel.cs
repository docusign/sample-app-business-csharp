using DocuSign.MyBusiness.Domain.Admin.Models;

namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class ResponseAccountStatusModel
    {
        public ConnectedUserModel ConnectedUser { get; set; }
        public bool IsConsentGranted { get; set; }
        public bool IsConnected { get; set; }
        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.UserAccount;
    }
}
