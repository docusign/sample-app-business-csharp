using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    [ExcludeFromCodeCoverage]
    public class DocuSignApiProvider : IDocuSignApiProvider
    {
        private readonly IDocuSignClientsFactory _docuSignClientsFactory;

        private Lazy<IEnvelopesApi> _envelopApi => new Lazy<IEnvelopesApi>(() => new EnvelopesApi(_apiClient.Value));
        private Lazy<ITemplatesApi> _templatesApi => new Lazy<ITemplatesApi>(() => new TemplatesApi(_apiClient.Value));
        private Lazy<IAccountsApi> _accountsApi => new Lazy<IAccountsApi>(() => new AccountsApi(_apiClient.Value));

        private Lazy<DocuSignClient> _apiClient => new Lazy<DocuSignClient>(() => _docuSignClientsFactory.BuildDocuSignApiClient());

        private Lazy<HttpClient> _docuSignHttpClient => new Lazy<HttpClient>(() => _docuSignClientsFactory.BuildHttpClient());

        public DocuSignApiProvider(IDocuSignClientsFactory docuSignClientsFactory)
        {
            _docuSignClientsFactory = docuSignClientsFactory;
        }

        public IEnvelopesApi EnvelopApi => _envelopApi.Value;
        public ITemplatesApi TemplatesApi => _templatesApi.Value;
        public IAccountsApi AccountsApi => _accountsApi.Value;

        public HttpClient DocuSignHttpClient => _docuSignHttpClient.Value;
    }
}
