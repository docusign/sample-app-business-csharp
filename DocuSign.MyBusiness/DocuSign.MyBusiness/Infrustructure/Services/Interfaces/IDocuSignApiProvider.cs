using System.Net.Http;
using DocuSign.eSign.Api;

namespace DocuSign.MyBusiness.Infrustructure.Services.Interfaces
{
    public interface IDocuSignApiProvider
    {
        IEnvelopesApi EnvelopApi { get; }
        ITemplatesApi TemplatesApi { get; }
        IAccountsApi AccountsApi { get; }

        HttpClient DocuSignHttpClient { get; }
    }
}