using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Infrustructure.Services.Interfaces
{
    public interface IAppConfiguration
    {
        AppConfigurationCustomerProfile CustomerProfile { get; set; }
        AppConfigurationDocuSign DocuSign { get; set; }
    }
}