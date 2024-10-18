using DocuSign.eSign.Client;
using System.Net.Http;

namespace DocuSign.MyBusiness.Infrustructure.Services.Interfaces
{
    public interface IDocuSignClientsFactory
    {
        HttpClient BuildHttpClient();
        DocuSignClient BuildDocuSignApiClient();
        DocuSignClient BuildDocuSignAuthClient(string authServer);
    }
}