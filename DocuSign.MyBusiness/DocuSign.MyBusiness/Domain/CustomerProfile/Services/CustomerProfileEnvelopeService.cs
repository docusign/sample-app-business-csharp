using System;
using System.Linq;
using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Models;
using DocuSign.MyBusiness.Domain.CustomerProfile.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.CustomerProfile.Services
{
    public class CustomerProfileEnvelopeService : ICustomerProfileEnvelopeService
    {
        private readonly ITemplateBuilder _templateBuilder;
        private readonly ICustomerProfileEnvelopeBuilder _envelopBuilder;
        private readonly IDocuSignApiProvider _docuSignApiProvider;
        private readonly IAppConfiguration _appConfiguration;

        private readonly string _eventNotificationUrl;

        public CustomerProfileEnvelopeService(
            ITemplateBuilder templateBuilder,
            ICustomerProfileEnvelopeBuilder envelopBuilder,
            IDocuSignApiProvider docuSignApiProvider,
            IAppConfiguration appConfiguration)
        {
            _templateBuilder = templateBuilder;
            _envelopBuilder = envelopBuilder;
            _docuSignApiProvider = docuSignApiProvider;
            _appConfiguration = appConfiguration;

            _eventNotificationUrl = $"{appConfiguration.DocuSign.EventNotificationBaseUrl}/api/callback/event";

        }

        public CreateEnvelopeResponse CreateUpdateProfileEnvelope(string accountId, string redirectUrl, CustomerProfileSignerInfo signerInfo)
        {
            var serverTemplateId = GetOrCreateServerTemplateId(
                TemplateNames.UserProfileUpdateTemplateName,
                () => _templateBuilder.BuildTemplate(TemplateNames.UserProfileUpdateTemplateName),
                accountId);

            string clientUserId = signerInfo.SignerEmail;

            AccountIdentityVerificationWorkflow identityWorkflow = GetIdentityPriorityWorkflow(accountId);

            var envelope = _envelopBuilder.BuildUserProfileUpdateEnvelope(
                _eventNotificationUrl,
                clientUserId,
                serverTemplateId,
                signerInfo,
                identityWorkflow);

            EnvelopeSummary envelopeSummary = _docuSignApiProvider.EnvelopApi.CreateEnvelope(accountId, envelope);

            ViewUrl recipientView = _docuSignApiProvider.EnvelopApi.CreateRecipientView(
                accountId,
                envelopeSummary.EnvelopeId,
                new RecipientViewRequest
                {
                    AuthenticationMethod = "Password",
                    AssertionId = $"{DateTime.UtcNow.ToString("s")}_Login_Logs",
                    AuthenticationInstant = DateTime.UtcNow.ToString("s"),
                    SecurityDomain = "mybusiness.com",
                    Email = signerInfo.SignerEmail,
                    UserName = signerInfo.SignerName,
                    ClientUserId = clientUserId,
                    ReturnUrl = $"{_appConfiguration.DocuSign.RedirectBaseUrl}/{redirectUrl}",

                });
            return new CreateEnvelopeResponse(recipientView.Url, envelopeSummary.EnvelopeId);
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

        private AccountIdentityVerificationWorkflow GetIdentityPriorityWorkflow(string accountId)
        {
            AccountIdentityVerificationResponse response =
                _docuSignApiProvider.AccountsApi.GetAccountIdentityVerification(accountId);
            return response.IdentityVerification.SingleOrDefault(x => x.WorkflowLabel == IdentityWorkflowNames.IDVerificationWorkflowLabel)
                ?? response.IdentityVerification.SingleOrDefault(x => x.WorkflowLabel == IdentityWorkflowNames.PhoneAuthWorkflowLabel)
                ?? throw new ApplicationException("Identity verification is not supported for the account");
        }
    }
}
