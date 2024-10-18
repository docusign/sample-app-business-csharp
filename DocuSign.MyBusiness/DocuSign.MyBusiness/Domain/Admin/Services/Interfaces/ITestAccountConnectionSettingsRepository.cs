using DocuSign.MyBusiness.Domain.Admin.Models;

namespace DocuSign.MyBusiness.Domain.Admin.Services.Interfaces
{
    public interface ITestAccountConnectionSettingsRepository
    {
        AccountConnectionSettings Get();
    }
}