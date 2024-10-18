using System;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.TermsAndConditions.Services
{
    public class TermsAndConditionsEnvelopeService : ITermsAndConditionsEnvelopeService
    {
        private readonly ITemplateBuilder _templateBuilder;
        private readonly ITermsAndConditionsEnvelopeBuilder _envelopBuilder;
        private readonly IDocuSignApiProvider _docuSignApiProvider;

        private readonly string _eventNotificationUrl;

        public TermsAndConditionsEnvelopeService(
            ITemplateBuilder templateBuilder,
            ITermsAndConditionsEnvelopeBuilder envelopBuilder,
            IDocuSignApiProvider docuSignApiProvider,
            IAppConfiguration appConfiguration)
        {
            _templateBuilder = templateBuilder;
            _envelopBuilder = envelopBuilder;
            _docuSignApiProvider = docuSignApiProvider;

            _eventNotificationUrl = $"{appConfiguration.DocuSign.EventNotificationBaseUrl}/api/callback/event";

        }

        public CreateEnvelopeResponse CreateTermsAndConditionsWithContractEnvelop(string accountId, SignerInfo signerInfo, CarbonCopyInfo carbonCopyInfo)
        {
            string contractTemplateId = GetOrCreateServerTemplateId(
                TemplateNames.ContractTemplateName,
                () => _templateBuilder.BuildTemplate(TemplateNames.ContractTemplateName),
                accountId);
            string termsAndConditionsTemplateId = GetOrCreateServerTemplateId(
                TemplateNames.TermsAndConditionsTemplateName,
                () => _templateBuilder.BuildTemplate(TemplateNames.TermsAndConditionsTemplateName),
                accountId);

            EnvelopeDefinition envelope = _envelopBuilder.BuildTermsAndConditionsWithContractEnvelope(
                _eventNotificationUrl,
                contractTemplateId,
                termsAndConditionsTemplateId,
                signerInfo,
                carbonCopyInfo);

            EnvelopeSummary envelopeSummary = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);

            return new CreateEnvelopeResponse(string.Empty, envelopeSummary.EnvelopeId);
        }

        private string GetOrCreateServerTemplateId(string templateName, Func<EnvelopeTemplate> buildTemplate, string accountId)
        {
            var listTemplates = _docuSignApiProvider.TemplatesApi.ListTemplates(accountId);
            EnvelopeTemplate template = listTemplates?.EnvelopeTemplates?.FirstOrDefault(x => x.Name == templateName);
            if (template != null)
            {
                return template.TemplateId;
            }

            EnvelopeTemplate envelopeTemplate = buildTemplate();
            TemplateSummary templateSummary = _docuSignApiProvider.TemplatesApi.CreateTemplate(accountId, envelopeTemplate);

            return templateSummary.TemplateId;
        }
    }
}
