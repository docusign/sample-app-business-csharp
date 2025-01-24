using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.MyBusiness.Controllers.Admin.Model;
using DocuSign.MyBusiness.Domain.Admin.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Exceptions;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

using IAuthenticationService = DocuSign.MyBusiness.Domain.Admin.Services.Interfaces.IAuthenticationService;

namespace DocuSign.MyBusiness.Domain.Common.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAppConfiguration _appConfiguration;
        private readonly IDocuSignClientsFactory _docuSignClientsFactory;
        private readonly ICustomerProfileRepository _customerProfileRepository;

        public AuthenticationService(
            IDocuSignClientsFactory docuSignClientsFactory,
            ICustomerProfileRepository customerProfileRepository,
            IAppConfiguration appConfiguration)
        {
            _docuSignClientsFactory = docuSignClientsFactory;
            _customerProfileRepository = customerProfileRepository;
            _appConfiguration = appConfiguration;
        }

        public string PrePopulateUserId(string basePath, string code)
        {
            string authServer = new Uri(basePath).Host;
            var apiClient = _docuSignClientsFactory.BuildDocuSignAuthClient(authServer);
            OAuth.OAuthToken authToken;
            if (authServer.Contains("-d")) // demo
            {
                authToken = apiClient.GenerateAccessToken(_appConfiguration.DocuSign.IntegrationKey, _appConfiguration.DocuSign.SecretKey, code);
            }
            else // prod
            {
                authToken = apiClient.GenerateAccessToken(_appConfiguration.DocuSign.IntegrationKey, _appConfiguration.DocuSign.SecretKeyProd, code);
            }
            var userInfo = apiClient.GetUserInfo(authToken.access_token);
            return userInfo.Sub;
        }

        public List<ResponseGetAccountsModel> GetAccounts(string basePath, string userId)
        {
            string authServer = new Uri(basePath).Host;
            var apiClient = _docuSignClientsFactory.BuildDocuSignAuthClient(authServer);
            OAuth.OAuthToken authToken = null;
            try
            {
                authToken = apiClient.RequestJWTUserToken(
                    _appConfiguration.DocuSign.IntegrationKey,
                    userId,
                    authServer,
                    File.ReadAllBytes(_appConfiguration.DocuSign.RSAPrivateKeyFile),
                    _appConfiguration.DocuSign.JWTLifeTime,
                    new List<string> { "impersonation", "signature" });
            }
            catch (ApiException ex)
            {
                throw new ApplicationApiException(ex);
            }

            var userInfo = apiClient.GetUserInfo(authToken.access_token);

            return userInfo.Accounts.Select(a => new ResponseGetAccountsModel
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                BaseUri = a.BaseUri,
                IsDefault = bool.Parse(a.IsDefault)
            }).ToList();
        }

        public ClaimsPrincipal AuthenticateFromJwt(AccountConnectionSettings accountConnectionSettings)
        {
            string authServer = new Uri(accountConnectionSettings.BasePath).Host;
            var apiClient = _docuSignClientsFactory.BuildDocuSignAuthClient(authServer);
            OAuth.OAuthToken authToken = null;
            try
            {
                authToken = apiClient.RequestJWTUserToken(
                    _appConfiguration.DocuSign.IntegrationKey,
                    accountConnectionSettings.UserId,
                    authServer,
                    File.ReadAllBytes(_appConfiguration.DocuSign.RSAPrivateKeyFile),
                    _appConfiguration.DocuSign.JWTLifeTime,
                    new List<string> { "impersonation", "signature" });
            }
            catch (ApiException ex)
            {
                throw new ApplicationApiException(ex);
            }

            var userInfo = apiClient.GetUserInfo(authToken.access_token);
            var account = userInfo.Accounts.SingleOrDefault(x => x.AccountId == accountConnectionSettings.AccountId);

            if (account == null)
            {
                throw ApplicationApiException.CreateInvalidAccountException();
            }

            if (account.BaseUri != accountConnectionSettings.BaseUri)
            {
                throw ApplicationApiException.CreateInvalidBaseUriException();
            }

            var claims = new List<Claim>
                {
                    new Claim("access_token", authToken.access_token),
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
                    new Claim(ClaimTypes.Name, userInfo.Name),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim("base_uri", account.BaseUri),
                    new Claim("account_name", account.AccountName),
                    new Claim("account_id", accountConnectionSettings.AccountId)
                };


            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }

        public string CreateAdminConsentUrl(string baseUrl, string redirectUrl)
        {
            string integrationKey = _appConfiguration.DocuSign.IntegrationKey;
            string scope = "openid signature impersonation";
            string adminConsentScope = "signature impersonation";

            string fullRedirectUrl = $"{_appConfiguration.DocuSign.RedirectBaseUrl}/{redirectUrl}";

            UriBuilder builder = new UriBuilder($"{baseUrl}/oauth/auth?response_type=code&scope={scope}&client_id={integrationKey}&redirect_uri={fullRedirectUrl}&admin_consent_scope={adminConsentScope}");
            return builder.ToString();
        }

        public string CreateUserConsentUrl(string baseUrl, string redirectUrl)
        {
            string integrationKey = _appConfiguration.DocuSign.IntegrationKey;
            string scope = "signature impersonation";
            string fullRedirectUrl = $"{_appConfiguration.DocuSign.RedirectBaseUrl}/{redirectUrl}";

            UriBuilder builder = new UriBuilder($"{baseUrl}/oauth/auth?response_type=code&scope={scope}&client_id={integrationKey}&redirect_uri={fullRedirectUrl}");
            return builder.ToString();
        }

        public void AuthenticateForProfileManagement(string login, string password)
        {
            var validLogin = _customerProfileRepository.Login == login;
            var validPassword = _customerProfileRepository.Password == password;

            switch ((validLogin, validPassword))
            {
                case (true, true):
                    return;
                case (true, false):
                    throw new AuthenticationException(ApiErrorDetails.InvalidPassword);
                case (false, true):
                case (false, false):
                    throw new AuthenticationException(ApiErrorDetails.InvalidAuthentication);
            }
        }

        public string FreeTrialPartnerIK()
        {
            return _appConfiguration.DocuSign.PartnerIK;
        }
    }
}
