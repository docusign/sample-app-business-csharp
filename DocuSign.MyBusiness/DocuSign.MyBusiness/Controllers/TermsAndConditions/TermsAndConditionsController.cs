using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Controllers.TermsAndConditions.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyBusiness.Controllers.TermsAndConditions
{
    [Authorize]
    public class TermsAndConditionsController : Controller
    {
        private readonly ITermsAndConditionsEnvelopeService _envelopeService;
        private readonly IAccountRepository _accountRepository;
        private readonly IEventsRepository _eventsRepository;

        public TermsAndConditionsController(
            ITermsAndConditionsEnvelopeService envelopeService,
            IAccountRepository accountRepository,
            IEventsRepository eventsRepository)
        {
            _envelopeService = envelopeService;
            _accountRepository = accountRepository;
            _eventsRepository = eventsRepository;
        }

        [HttpPost]
        [Route("/api/envelopes/terms-and-conditions-with-contract")]
        public IActionResult CreateTermsAndConditionsWithContractEnvelope([FromBody] RequestTermsAndConditionsWithContractEnvelopeModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }

            CreateEnvelopeResponse createEnvelopeResponse =
                _envelopeService.CreateTermsAndConditionsWithContractEnvelop(
                        _accountRepository.AccountId,
                        new SignerInfo
                        {
                            Email = model.SignerInfo.Email,
                            FullName = $"{model.SignerInfo.FirstName} {model.SignerInfo.LastName}",
                            AccessCode = model.SignerInfo.AccessCode
                        },
                        new CarbonCopyInfo
                        {
                            Email = model.CarbonCopyInfo.Email,
                            FullName = model.CarbonCopyInfo.FullName
                        });

            _eventsRepository.SaveEnvelope(
                _accountRepository.AccessToken,
                createEnvelopeResponse.EnvelopeId,
                UseCaseTypes.TermsAndConditions);

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });
        }
    }
}