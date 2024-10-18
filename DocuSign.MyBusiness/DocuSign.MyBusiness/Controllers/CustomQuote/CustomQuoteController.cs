using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Controllers.CustomQuote.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyBusiness.Controllers.CustomQuote
{
    [Authorize]
    public class CustomQuoteController : Controller
    {
        private readonly ICustomQuoteEnvelopeService _envelopeService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerProfileRepository _customerProfileRepository;
        private readonly IEventsRepository _eventsRepository;

        public CustomQuoteController(
            ICustomQuoteEnvelopeService envelopeService,
            IAccountRepository accountRepository,
            IEventsRepository eventsRepository,
            ICustomerProfileRepository customerProfileRepository)
        {
            _envelopeService = envelopeService;
            _accountRepository = accountRepository;
            _eventsRepository = eventsRepository;
            _customerProfileRepository = customerProfileRepository;
        }

        [HttpPost]
        [Route("/api/envelopes/custom-quote")]
        public IActionResult CreateCustomQuoteEnvelope([FromBody] RequestCustomQuoteEnvelopeModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }

            CreateEnvelopeResponse createEnvelopeResponse =
                _envelopeService.CreateCustomQuoteEnvelop(
                        _accountRepository.AccountId,
                        new SignerInfo
                        {
                            FullName = $"{model.FirstName} {model.LastName}",
                            CountryCode = model.CountryCode,
                            PhoneNumber = model.PhoneNumber,
                        });

            _eventsRepository.SaveEnvelope(
                _accountRepository.AccessToken,
                createEnvelopeResponse.EnvelopeId,
                UseCaseTypes.CustomQuote);

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });
        }
    }
}