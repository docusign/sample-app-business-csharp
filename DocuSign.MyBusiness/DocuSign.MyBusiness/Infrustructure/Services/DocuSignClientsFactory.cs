using DocuSign.eSign.Client;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    public class DocuSignClientsFactory : IDocuSignClientsFactory
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpClientFactory _clientFactory;

        public DocuSignClientsFactory(IAccountRepository accountRepository, IHttpClientFactory clientFactory)
        {
            _accountRepository = accountRepository;
            _clientFactory = clientFactory;
        }

        public DocuSignClient BuildDocuSignApiClient()
        {
            var docuSignConfig = new Configuration(_accountRepository.BaseUri + "/restapi");
            docuSignConfig.AddDefaultHeader("Authorization", "Bearer " + _accountRepository.AccessToken);
            docuSignConfig.AccessToken = _accountRepository.AccessToken;
            var apiClient = new DocuSignClient(docuSignConfig);
            apiClient.SetBasePath(docuSignConfig.BasePath);
            return apiClient;
        }

        public DocuSignClient BuildDocuSignAuthClient(string authServer)
        {
            var client = new DocuSignClient();
            client.SetOAuthBasePath(authServer);
            return client;
        }

        public HttpClient BuildHttpClient()
        {
            HttpClient client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accountRepository.AccessToken);
            client.BaseAddress = new Uri(_accountRepository.BaseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
