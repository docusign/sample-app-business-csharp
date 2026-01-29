using DocumentFormat.OpenXml.EMMA;
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
using System;
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
            if (model == null)
            {
                return BadRequest();
            }

            if (!TryNormalizeUri(model.BasePath, out var normalizedBasePath))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidBasePath,
                    model,
                    m => m.BasePath));
            }

            var settings = _settingsRepository.Get();
            settings.BasePath = normalizedBasePath;
            _settingsRepository.Save(settings);

            switch (model.ConsentType)
            {
                case ConsentType.Admin:
                    return Ok(new ResponseAccountAuthorizeModel(_authenticationService.CreateAdminConsentUrl(normalizedBasePath, $"api/consentcallback")));
                case ConsentType.Individual:
                    return Ok(new ResponseAccountAuthorizeModel(_authenticationService.CreateUserConsentUrl(normalizedBasePath, $"api/consentcallback")));
                default:
                    return BadRequest("Unknown consent type");
            }
        }

        [Authorize]
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

        [Authorize]
        [HttpGet]
        [Route("/api/accounts")]
        public IActionResult GetAccounts(string basePath, string userId)
        {
            try
            {
                if (!TryNormalizeUri(basePath, out var normalizedBasePath))
                {
                    return BadRequest(ApiErrorDetails.InvalidBasePath);
                }

                if (!TryNormalizeIdentifier(userId, out var normalizedUserId))
                {
                    return BadRequest(ApiErrorDetails.InvalidUserId);
                }

                var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!TryNormalizeIdentifier(currentUserId, out var normalizedCurrentUserId) ||
                    !string.Equals(normalizedUserId, normalizedCurrentUserId, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                var result = _authenticationService.GetAccounts(normalizedBasePath, normalizedUserId);
                return Ok(result);
            }
            catch (ApplicationApiException ex)
            {
                return BadRequest(ex.Details.Error);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/api/account/connect")]
        public async Task<IActionResult> Connect([FromBody] RequestAccountConnectModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!TryNormalizeIdentifier(model.UserId, out var normalizedUserId))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidUserId,
                    model,
                    m => m.UserId));
            }

            if (!TryNormalizeIdentifier(model.AccountId, out var normalizedAccountId))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidAccountId,
                    model,
                    m => m.AccountId));
            }

            if (!TryNormalizeUri(model.BasePath, out var normalizedBasePath))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidBasePath,
                    model,
                    m => m.BasePath));
            }

            if (!TryNormalizeUri(model.BaseUri, out var normalizedBaseUri))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidBaseUri,
                    model,
                    m => m.BaseUri));
            }

            model.UserId = normalizedUserId;
            model.AccountId = normalizedAccountId;
            model.BasePath = normalizedBasePath;
            model.BaseUri = normalizedBaseUri;

            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!TryNormalizeIdentifier(currentUserId, out var normalizedCurrentUserId) ||
                !string.Equals(model.UserId, normalizedCurrentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            try
            {
                var accounts = _authenticationService.GetAccounts(model.BasePath, normalizedCurrentUserId);
                var hasAccess = accounts.Any(a =>
                    string.Equals(a.AccountId, model.AccountId, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(a.BaseUri, model.BaseUri, StringComparison.OrdinalIgnoreCase));

                if (!hasAccess)
                {
                    return Forbid();
                }
            }
            catch (ApplicationApiException ex)
            {
                return BadRequest(CreateErrorDetails(ex.Details, model));
            }

            AccountConnectionSettings connectionSettings = CreateConnectionSettings(model);
            try
            {
                ClaimsPrincipal principal =
                    _authenticationService.AuthenticateFromJwt(connectionSettings);

                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
            var model = new ResponseAccountStatusModel
            {
                ConnectedUser = new ConnectedUserModel
                {
                    Name = HttpContext.User.Identity.Name ?? string.Empty,
                    Email = _accountRepository.Email,
                    AccountName = _accountRepository.AccountName
                },
                IsConsentGranted = _settingsRepository.Get().IsConsentGranted,
                IsConnected = HttpContext.User.Identity.IsAuthenticated
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPut]
        [Route("/api/settings")]
        public IActionResult SetSettings([FromBody] SettingsModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!TryNormalizeIdentifier(model.UserId, out var normalizedUserId))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidUserId,
                    model,
                    m => m.UserId));
            }

            if (!TryNormalizeIdentifier(model.AccountId, out var normalizedAccountId))
            {
                return BadRequest(ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                    ApiErrorDetails.InvalidAccountId,
                    model,
                    m => m.AccountId));
            }

            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!TryNormalizeIdentifier(currentUserId, out var normalizedCurrentUserId) ||
                !string.Equals(normalizedUserId, normalizedCurrentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var currentAccountId = HttpContext.User.FindFirstValue("account_id");
            if (!TryNormalizeIdentifier(currentAccountId, out var normalizedCurrentAccountId) ||
                !string.Equals(normalizedAccountId, normalizedCurrentAccountId, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var settings = _settingsRepository.Get();
            settings.BaseUri = settings.BaseUri;
            settings.UserId = normalizedUserId;
            settings.AccountId = normalizedAccountId;
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

        private const int MaxIdLength = 128;
        private const int MaxUriLength = 512;

        private bool TryNormalizeIdentifier(string value, out string normalized)
        {
            normalized = value?.Trim();
            if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxIdLength)
            {
                return false;
            }

            foreach (var ch in normalized)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '-')
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryNormalizeUri(string value, out string normalized)
        {
            normalized = value?.Trim();
            if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > MaxUriLength)
            {
                return false;
            }

            if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
            {
                return false;
            }

            if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            {
                return false;
            }

            normalized = uri.ToString().TrimEnd('/');
            return true;
        }
    }
}