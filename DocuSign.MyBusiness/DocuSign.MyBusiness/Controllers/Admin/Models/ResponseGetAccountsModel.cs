namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class ResponseGetAccountsModel
    {
        public string BaseUri { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; } 
        public bool IsDefault { get; set; }
    }
}

