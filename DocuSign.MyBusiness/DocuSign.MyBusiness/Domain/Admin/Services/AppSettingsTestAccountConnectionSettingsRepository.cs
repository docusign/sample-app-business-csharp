using DocuSign.MyBusiness.Domain.Admin.Models;
using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.Admin.Services
{
    public class AppSettingsTestAccountConnectionSettingsRepository : ITestAccountConnectionSettingsRepository
    {
        IAppConfiguration _appConfiguration;

        public AppSettingsTestAccountConnectionSettingsRepository(IAppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        public AccountConnectionSettings Get()
        {
            return new AccountConnectionSettings
            {
                BasePath = _appConfiguration.DocuSign.TestAccountConnectionSettings.BasePath,
                BaseUri = _appConfiguration.DocuSign.TestAccountConnectionSettings.BaseUri,
                UserId = _appConfiguration.DocuSign.TestAccountConnectionSettings.UserId,
                AccountId = _appConfiguration.DocuSign.TestAccountConnectionSettings.AccountId,
            };
        }
    }
}
