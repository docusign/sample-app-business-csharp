using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Controllers.Admin.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Admin.Models;
using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Exceptions;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IAuthenticationService = DocuSign.MyBusiness.Domain.Admin.Services.Interfaces.IAuthenticationService;

namespace DocuSign.MyBusiness.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly ITestAccountConnectionSettingsRepository _testAccountConnectionSettingsRepository;

        public AdminController(
            IAuthenticationService authenticationService,
            ISettingsRepository settingsRepository,
            IAccountRepository accountRepository,
            IDocuSignApiProvider docuSignApiProvider,
            ITestAccountConnectionSettingsRepository testAccountConnectionSettingsRepository)
        {
            _authenticationService = authenticationService;
            _settingsRepository = settingsRepository;
            _docuSignApiProvider = docuSignApiProvider;
            _accountRepository = accountRepository;
            _testAccountConnectionSettingsRepository = testAccountConnectionSettingsRepository;
        }

        [HttpPost]
        [Route("/api/account/consent/obtain")]
        public IActionResult ObtainConsent([FromBody] RequestAccountAuthorizeModel model)
        {
            var settings = _settingsRepository.Get();
            settings.BasePath = model.BasePath;
            _settingsRepository.Save(settings);

            switch (model.ConsentType)
            {
                case ConsentType.Admin:
                    return Ok(new ResponseAccountAuthorizeModel(_authenticationService.CreateAdminConsentUrl(model.BasePath, $"api/consentcallback")));
                case ConsentType.Individual:
                    return Ok(new ResponseAccountAuthorizeModel(_authenticationService.CreateUserConsentUrl(model.BasePath, $"api/consentcallback")));
                default:
                    return BadRequest("Unknown consent type");
            }
        }

        [HttpGet]
        [Route("/api/account/consent/remove")]
        public IActionResult RemoveConsent()
        {
            var settings = _settingsRepository.Get();
            settings.IsConsentGranted = false;
            _settingsRepository.Save(settings);
            return NoContent();
        }

        [HttpGet]
        [Route("/api/consentcallback")]
        public IActionResult ConsentCallback(string code)
        {
            var settings = _settingsRepository.Get();
            settings.IsConsentGranted = true;
            settings.UserId = _authenticationService.PrePopulateUserId(settings.BasePath, code);
            _settingsRepository.Save(settings);
            return LocalRedirect("/admin");
        }

        [HttpGet]
        [Route("/api/accounts")]
        public IActionResult GetAccounts(string basePath, string userId)
        {
            try
            {
                var result = _authenticationService.GetAccounts(basePath, userId);
                return Ok(result);
            }
            catch (ApplicationApiException ex)
            {
                return BadRequest(ex.Details.Error);
            }
        }

        [HttpPost]
        [Route("/api/account/connect")]
        public async Task<IActionResult> Connect([FromBody] RequestAccountConnectModel model)
        {
            AccountConnectionSettings connectionSettings = CreateConnectionSettings(model);
            try
            {
                ClaimsPrincipal principal =
                    _authenticationService.AuthenticateFromJwt(connectionSettings);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);
                HttpContext.User = principal;

                var settings = _settingsRepository.Get();
                settings.AuthenticationType = model.AuthenticationType;
                settings.UserId = model.UserId;
                settings.AccountId = model.AccountId;
                settings.BaseUri = model.BaseUri;
                settings.TemplatesDataSource = GetTemplatesDataSource(connectionSettings.AccountId);
                settings.Template = TemplateNames.DefaultTemplateId;
                settings.SignatureTypesDataSource = GetSignatureTypesDataSource(connectionSettings.AccountId);
                settings.SignatureType = SignatureInfo.DefaultProviderName;
                settings.IsConsentGranted = true;
                _settingsRepository.Save(settings);
            }
            catch (ApplicationApiException ex)
            {
                return BadRequest(CreateErrorDetails(ex.Details, model));
            }

            return NoContent();
        }

        [HttpGet]
        [Route("/api/account/status")]
        public IActionResult GetStatus()
        {
            var settings = _settingsRepository.Get();
            var model = new ResponseAccountStatusModel
            {
                ConnectedUser = new ConnectedUserModel
                {
                    Name = HttpContext.User.Identity.Name ?? string.Empty,
                    Email = _accountRepository.Email,
                    AccountName = _accountRepository.AccountName
                },
                IsConsentGranted = settings.IsConsentGranted,
                IsConnected = HttpContext.User.Identity.IsAuthenticated,
                AuthenticationType = settings.AuthenticationType,
            };
            return Ok(model);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/account/disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            await HttpContext.SignOutAsync();
            var settings = _settingsRepository.Get();
            settings.Template = null;
            settings.SignatureType = null;
            settings.TemplatesDataSource = null;
            settings.SignatureTypesDataSource = null;
            settings.AuthenticationType = AuthenticationType.None;
            _settingsRepository.Save(settings);
            return NoContent();
        }

        [Authorize]
        [HttpGet]
        [Route("/api/account/logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("/api/settings")]
        public IActionResult GetSetting()
        {
            Settings settings = _settingsRepository.Get();
            return Ok(new SettingsModel
            {
                BasePath = settings.BasePath,
                BaseUri = settings.BaseUri,
                AccountId = settings.AccountId,
                UserId = settings.UserId,
                Template = settings.Template,
                SignatureType = settings.SignatureType,
                UserProfile = settings.UserProfile
            });
        }

        [HttpGet]
        [Route("/api/settings/datasource")]
        public IActionResult GetDatasource()
        {
            Settings settings = _settingsRepository.Get();
            return Ok(new DataSourceModel
            {
                SignatureTypes = settings.SignatureTypesDataSource,
                Templates = settings.TemplatesDataSource,
            });
        }

        [HttpPut]
        [Route("/api/settings")]
        public IActionResult SetSettings([FromBody] SettingsModel model)
        {
            var settings = _settingsRepository.Get();
            settings.BaseUri = settings.BaseUri;
            settings.UserId = model.UserId;
            settings.AccountId = model.AccountId;
            settings.Template = model.Template;
            settings.SignatureType = model.SignatureType;
            settings.UserProfile = model.UserProfile;
            _settingsRepository.Save(settings);

            return Ok(model);
        }

        private AccountConnectionSettings CreateConnectionSettings(RequestAccountConnectModel model)
        {
            AccountConnectionSettings connectionSettings = null;
            switch (model.AuthenticationType)
            {
                case AuthenticationType.UserAccount:
                    connectionSettings = new AccountConnectionSettings
                    {
                        BasePath = model.BasePath,
                        BaseUri = model.BaseUri,
                        AccountId = model.AccountId,
                        UserId = model.UserId
                    };
                    break;
                case AuthenticationType.TestAccount:
                    connectionSettings = _testAccountConnectionSettingsRepository.Get();
                    break;
                default:
                    break;
            }

            return connectionSettings;
        }

        private ErrorDetailsModel CreateErrorDetails(ApiErrorDetails error, RequestAccountConnectModel model)
        {
            switch (error.Error)
            {
                case ApiErrorDetails.InvalidBasePath:
                    return ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(error.Error, model, model => model.BasePath);
                case ApiErrorDetails.InvalidBaseUri:
                    return ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(error.Error, model, model => model.BaseUri);
                case ApiErrorDetails.InvalidUserId:
                    return ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(error.Error, model, model => model.UserId);
                case ApiErrorDetails.InvalidAccountId:
                    return ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(error.Error, model, model => model.AccountId);
                default:
                    return ErrorDetailsModel.CreateGeneralErrorDetails(error.Error);
            }
        }

        private IEnumerable<DataSourceItem> GetSignatureTypesDataSource(string accountId)
        {
            var signatureProviders = _docuSignApiProvider.AccountsApi.ListSignatureProviders(accountId);
            var result = new List<DataSourceItem>
            {
                new DataSourceItem
                {
                    Key = SignatureInfo.DefaultProviderName,
                    Value = SignatureInfo.DefaultProviderDisplayName
                }
            };

            result.AddRange(signatureProviders.SignatureProviders.Select(p => new DataSourceItem(p.SignatureProviderName, p.SignatureProviderDisplayName)));
            return result;
        }

        private IEnumerable<DataSourceItem> GetTemplatesDataSource(string accountId)
        {
            EnvelopeTemplateResults userTemplates = _docuSignApiProvider.TemplatesApi.ListTemplates(accountId);
            var result = new List<DataSourceItem>
            {
                new DataSourceItem
                {
                    Key = TemplateNames.DefaultTemplateId,
                    Value = TemplateNames.DefaultTemplateName
                }
            };
            result.AddRange(userTemplates.EnvelopeTemplates.Select(t => new DataSourceItem(t.TemplateId, t.Name)));
            return result;
        }
    }
}