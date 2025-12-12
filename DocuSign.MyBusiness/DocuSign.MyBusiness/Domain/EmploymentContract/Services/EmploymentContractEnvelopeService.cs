using System;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.EmploymentContract.Services
{
    public class EmploymentContractEnvelopeService : IEmploymentContractEnvelopeService
    {
        private readonly ITemplateBuilder _templateBuilder;
        private readonly IEmploymentContractEnvelopeBuilder _envelopBuilder;
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IAppConfiguration _appConfiguration;

        private readonly string _eventNotificationUrl;

        public EmploymentContractEnvelopeService(
            ITemplateBuilder templateBuilder,
            IEmploymentContractEnvelopeBuilder envelopBuilder,
            IDocuSignApiProvider docuSignApiProvider,
            IAppConfiguration appConfiguration)
        {
            _templateBuilder = templateBuilder;
            _envelopBuilder = envelopBuilder;
            _docuSignApiProvider = docuSignApiProvider;
            _appConfiguration = appConfiguration;

            _eventNotificationUrl = $"{appConfiguration.DocuSign.EventNotificationBaseUrl}/api/callback/event";

        }

        public CreateEnvelopeResponse CreateEmploymentContractEnvelop(EnvelopeAction envelopAction, string templateId, string accountId, string redirectUrl, SignerInfo signerInfo, SignatureInfo signatureInfo)
        {
            var serverTemplateId = GetServerTemplateId(templateId, TemplateNames.EmploymentContractTemplateName, accountId);

            var envelope = _envelopBuilder.BuildEmploymentContractEnvelope(
                _eventNotificationUrl,
                envelopAction,
                serverTemplateId,
                signerInfo,
                signatureInfo);

            EnvelopeSummary envelopeSummary = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);

            switch (envelopAction)
            {
                case EnvelopeAction.Send:
                    return new CreateEnvelopeResponse(string.Empty, envelopeSummary.EnvelopeId);
                case EnvelopeAction.ReviewAndSend:
                    ViewUrl senderView = _docuSignApiProvider.EnvelopApi.CreateSenderView(
                        accountId,
                        envelopeSummary.EnvelopeId,
                        new EnvelopeViewRequest
                        {
                            ReturnUrl = $"{_appConfiguration.DocuSign.RedirectBaseUrl}/{redirectUrl}"
                        });
                    return new CreateEnvelopeResponse(senderView.Url, envelopeSummary.EnvelopeId);
                default:
                    throw new ApplicationException("The Envelop Action is not supported");
            }

        }

        private string GetServerTemplateId(string templateId, string templateName, string accountId)
        {
            if (templateId == TemplateNames.DefaultTemplateId)
            {
                return GetOrCreateServerTemplateId(
                    templateName,
                    () => _templateBuilder.BuildTemplate(templateName),
                    accountId);
            }

            return templateId;
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
