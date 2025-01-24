namespace DocuSign.MyBusiness.Infrustructure.Model
{
    public class AppConfigurationDocuSign
    {
        public string IntegrationKey { get; set; }
        public string SecretKey { get; set; }
        public string SecretKeyProd { get; set; }
        public string RedirectBaseUrl { get; set; }
        public string EventNotificationBaseUrl { get; set; }
        public string RSAPrivateKeyFile { get; set; }
        public int JWTLifeTime { get; set; }
        public AppSettingTestAccount TestAccountConnectionSettings { get; set; }
        public string PartnerIK { get; set; } 
    }
}
