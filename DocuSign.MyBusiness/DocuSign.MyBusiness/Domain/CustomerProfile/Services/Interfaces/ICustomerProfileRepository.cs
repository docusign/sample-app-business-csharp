using DocuSign.MyBusiness.Domain.CustomerProfile.Models;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces
{
    public interface ICustomerProfileRepository
    {
        string Login { get; }
        string Password { get; }
        CustomerProfileSignerInfo ProfileSignerInfo { get; }
    }
}