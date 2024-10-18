using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    public class ClaimsAccountRepository : IAccountRepository
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ClaimsAccountRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string AccountId
        {
            get
            {
                return FindFirstValue("account_id");
            }
        }

        public string BaseUri
        {
            get
            {
                return FindFirstValue("base_uri");
            }
        }

        public string AccessToken
        {
            get
            {
                return FindFirstValue("access_token");
            }
        }

        public string Email
        {
            get
            {
                return FindFirstValue(ClaimTypes.Email);
            }

        }

        public string AccountName
        {
            get
            {
                return FindFirstValue("account_name");
            }
        }

        private string FindFirstValue(string key)
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(key) ?? string.Empty;
        }
    }
}
