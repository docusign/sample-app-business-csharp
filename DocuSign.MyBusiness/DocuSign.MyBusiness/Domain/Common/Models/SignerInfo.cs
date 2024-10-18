namespace DocuSign.MyBusiness.Domain.Common.Models
{
    public class SignerInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AccessCode { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}