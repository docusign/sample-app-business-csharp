using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Controllers.Common.Models;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.CustomQuote.Services.Interfaces;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Newtonsoft.Json;

namespace DocuSign.MyBusiness.Domain.CustomQuote.Services
{
    public class CustomQuoteEnvelopeService : ICustomQuoteEnvelopeService
    {
        private readonly ITemplateBuilder _templateBuilder;
        private readonly ICustomQuoteEnvelopeBuilder _envelopBuilder;
        private readonly IDocuSignApiProvider _docuSignApiProvider;

        private readonly string _eventNotificationUrl;

        public CustomQuoteEnvelopeService(
            ITemplateBuilder templateBuilder,
            ICustomQuoteEnvelopeBuilder envelopBuilder,
            IDocuSignApiProvider docuSignApiProvider,
            IAppConfiguration appConfiguration)
        {
            _templateBuilder = templateBuilder;
            _envelopBuilder = envelopBuilder;
            _docuSignApiProvider = docuSignApiProvider;

            _eventNotificationUrl = $"{appConfiguration.DocuSign.EventNotificationBaseUrl}/api/callback/event";

        }

        public CreateEnvelopeResponse CreateCustomQuoteEnvelop(string accountId, SignerInfo signerInfo)
        {
            var envelope = _envelopBuilder.BuildCustomQuoteEnvelope(_eventNotificationUrl, signerInfo);

            byte[] templateContent = _templateBuilder.GetBinaryContent(TemplateNames.CustomQuoteTemplateName);

            using MultipartFormDataContent multipartContent = new("---BOUNDARY");

            var envelopePart = new StringContent(
                JsonConvert.SerializeObject(envelope, Formatting.Indented),
                Encoding.ASCII,
                "application/json");

            multipartContent.Add(envelopePart);

            var templateContentPart = new ByteArrayContent(templateContent);
            templateContentPart.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Pdf);
            templateContentPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("file");
            templateContentPart.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("documentid", envelope.CompositeTemplates[0].Document.DocumentId));
            templateContentPart.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("name", envelope.CompositeTemplates[0].Document.Name));
            templateContentPart.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", "Order Form.pdf"));
            templateContentPart.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("compositeTemplateId", envelope.CompositeTemplates[0].CompositeTemplateId));

            multipartContent.Add(templateContentPart);

            using var response = _docuSignApiProvider.DocuSignHttpClient.PostAsync($"restapi/v2.1/accounts/{accountId}/envelopes", multipartContent).Result;
            string content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var result = JsonConvert.DeserializeObject<EnvelopeSummary>(content);

                return new CreateEnvelopeResponse(string.Empty, result.EnvelopeId);
            }
            else
            {
                throw new ApplicationException(content);
            }
        }

    }
}
