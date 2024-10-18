using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System;
using System.Security.Authentication;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Controllers.CustomerProfile.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;

namespace DocuSign.MyBusiness.Controllers.CustomerProfile
{
    [Authorize]
    public class CustomerProfileController : Controller
    {
        private readonly ICustomerProfileEnvelopeService _envelopeService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerProfileRepository _customerProfileRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IAuthenticationService _authenticationService;

        public CustomerProfileController(
            ICustomerProfileEnvelopeService envelopeService,
            IAccountRepository accountRepository,
            IEventsRepository eventsRepository,
            ICustomerProfileRepository customerProfileRepository,
            IAuthenticationService authenticationService)
        {
            _envelopeService = envelopeService;
            _accountRepository = accountRepository;
            _eventsRepository = eventsRepository;
            _customerProfileRepository = customerProfileRepository;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("/api/envelopes/update-profile")]
        public IActionResult CreateUpdateProfileEnvelope([FromBody] RequestUpdateProfileEnvelopeModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }

            CustomerProfileSignerInfo profileInfo = _customerProfileRepository.ProfileSignerInfo;

            CreateEnvelopeResponse createEnvelopeResponse =
                _envelopeService.CreateUpdateProfileEnvelope(
                        _accountRepository.AccountId,
                        model.RedirectUrl,
                        profileInfo);

            _eventsRepository.SaveEnvelope(
                _accountRepository.AccessToken,
                createEnvelopeResponse.EnvelopeId,
                UseCaseTypes.UpdateProfile);

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });

        }

        [HttpPost]
        [Route("/api/profile/authorize")]
        public IActionResult Authorize([FromBody] ProfileAuthenticationModel model)
        {
            try
            {
                _authenticationService.AuthenticateForProfileManagement(model.Login, model.Password);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(CreateErrorDetails(ex, model));
            }

            return NoContent();
        }

        [HttpGet]
        [Route("/api/profile")]
        public IActionResult GetProfile()
        {
            var profile = _customerProfileRepository.ProfileSignerInfo;
            var model = new CustomerProfileModel
            {
                FullName = profile.SignerName,
                Email = profile.SignerEmail,
                CountryCode = profile.CountryCode,
                PhoneNumber = profile.SignerPhone
            };
            return Ok(model);
        }
        private ErrorDetailsModel CreateErrorDetails(AuthenticationException ex, ProfileAuthenticationModel model)
        {
            switch (ex.Message)
            {
                case ApiErrorDetails.InvalidAuthentication:
                    return ErrorDetailsModel.CreateErrorDetailsForManyModelProperty(
                        ex.Message,
                        model,
                        new (string propertyErrorCode, Expression<Func<ProfileAuthenticationModel, string>> property)[]
                        {
                            (ApiErrorDetails.InvalidUserName, x => x.Login),
                            (ApiErrorDetails.InvalidPassword, x => x.Password)
                        });
                case ApiErrorDetails.InvalidPassword:
                    return ErrorDetailsModel.CreateErrorDetailsForOneModelProperty(
                        ex.Message,
                        model,
                        x => x.Password);
                default:
                    return ErrorDetailsModel.CreateGeneralErrorDetails(ex.Message);
            }
        }
    }
}