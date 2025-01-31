using System.Collections.Generic;
using System.Linq;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Domain.Admin.Models
{
    public class Settings
    {
        public Settings()
        {
            IsConsentGranted = false;
            TemplatesDataSource = Enumerable.Empty<DataSourceItem>();
            SignatureTypesDataSource = Enumerable.Empty<DataSourceItem>();
            UserProfile = new UserProfileSettings();
        }

        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.UserAccount;
        public string BasePath { get; set; }
        public string BaseUri { get; set; }
        public string AccountId { get; set; }
        public string UserId { get; set; }
        public bool IsConsentGranted { get; set; }
        public string Template { get; set; }
        public string SignatureType { get; set; }
        public IEnumerable<DataSourceItem> TemplatesDataSource { get; set; }
        public IEnumerable<DataSourceItem> SignatureTypesDataSource { get; set; }
        public UserProfileSettings UserProfile { get; set; }
    }
}
