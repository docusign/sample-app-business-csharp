using AutoFixture;

namespace DocuSign.MyBusiness.Infrustructure.Model
{
    public class SignatureInfo
    {
        public const string DefaultProviderName = "default_provider";
        public const string DefaultProviderDisplayName = "Default";

        public const string DsEmailProviderName = "ds_email";
        public const string DsElectronicProviderName = "universalsignaturepen_imageonly";
        public const string DsEuAdvancedProviderName = "universalsignaturepen_opentrust_hash_tsp";

        public string ProviderName { get; set; }
        public string AccessCode { get; set; }
    }
}