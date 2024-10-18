namespace DocuSign.MyBusiness.Infrustructure.Services.Interfaces
{
    public interface IAccountRepository
    {
        string AccessToken { get; }
        string AccountId { get; }
        string BaseUri { get; }
        public string Email { get; }
        public string AccountName { get; }
    }
}