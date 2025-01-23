using System.Collections.Generic;
using System.Security.Claims;
using DocuSign.MyBusiness.Controllers.Admin.Model;
using DocuSign.MyBusiness.Domain.Admin.Models;

namespace DocuSign.MyBusiness.Domain.Admin.Services.Interfaces
{
    public interface IAuthenticationService
    {
        ClaimsPrincipal AuthenticateFromJwt(AccountConnectionSettings connectionSettings);
        string CreateAdminConsentUrl(string baseUrl, string redirectUrl);
        string CreateUserConsentUrl(string baseUrl, string redirectUrl);
        void AuthenticateForProfileManagement(string login, string password);
        string PrePopulateUserId(string basePath, string code);
        List<ResponseGetAccountsModel> GetAccounts(string basePath, string userId);
        string FreeTrialPartnerIK();
    }
}