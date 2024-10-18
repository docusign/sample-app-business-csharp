using System.Collections.Generic;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Controllers.Events.Model
{
    public class SignerModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<DataSourceItem> Tabs { get; set; }
    }
}
