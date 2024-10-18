using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    public class AppSettingsConfiguration : IAppConfiguration
    {
        public AppSettingsConfiguration(IConfiguration configuration)
        {
            DocuSign = new AppConfigurationDocuSign
            {
                IntegrationKey = configuration["DocuSign:IntegrationKey"],
                SecretKey = configuration["DocuSign:SecretKey"],
                RedirectBaseUrl = configuration["DocuSign:RedirectBaseUrl"],
                EventNotificationBaseUrl = configuration["DocuSign:EventNotificationBaseUrl"],
                RSAPrivateKeyFile = configuration["DocuSign:RSAPrivateKeyFile"],
                JWTLifeTime = int.Parse(configuration["DocuSign:JWTLifeTime"]),
                TestAccountConnectionSettings = new AppSettingTestAccount
                {
                    BasePath = configuration["DocuSign:TestAccountConnectionSettings:BasePath"],
                    BaseUri = configuration["DocuSign:TestAccountConnectionSettings:BaseUri"],
                    UserId = configuration["DocuSign:TestAccountConnectionSettings:UserId"],
                    AccountId = configuration["DocuSign:TestAccountConnectionSettings:AccountId"],
                }
            };
            CustomerProfile = new AppConfigurationCustomerProfile
            {
                Login = configuration["CustomerProfile:Login"],
                Password = configuration["CustomerProfile:Password"],
            };
        }

        public AppConfigurationDocuSign DocuSign { get; set; }
        public AppConfigurationCustomerProfile CustomerProfile { get; set; }
    }
}
