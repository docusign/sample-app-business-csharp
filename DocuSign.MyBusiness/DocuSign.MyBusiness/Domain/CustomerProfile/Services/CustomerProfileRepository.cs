using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services
{
    public class CustomerProfileRepository : ICustomerProfileRepository
    {
        private readonly ISettingsRepository _settingsRepository;

        public CustomerProfileRepository(IAppConfiguration appConfiguration, ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;

            Login = appConfiguration.CustomerProfile.Login;
            Password = appConfiguration.CustomerProfile.Password;
        }

        public string Login { get; private set; }
        public string Password { get; private set; }
        public CustomerProfileSignerInfo ProfileSignerInfo
        {
            get
            {
                var settings = _settingsRepository.Get();
                return new CustomerProfileSignerInfo
                {
                    SignerName = settings.UserProfile.FullName,
                    SignerEmail = settings.UserProfile.Email,
                    CountryCode = settings.UserProfile.CountryCode,
                    SignerPhone = settings.UserProfile.PhoneNumber
                };
            }
        }
    }
}