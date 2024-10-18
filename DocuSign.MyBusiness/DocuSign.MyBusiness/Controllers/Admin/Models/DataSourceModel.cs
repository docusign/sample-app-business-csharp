using System.Collections.Generic;
using DocuSign.MyBusiness.Infrustructure.Model;

namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class DataSourceModel
    {
        public IEnumerable<DataSourceItem> Templates { get; set; }
        public IEnumerable<DataSourceItem> SignatureTypes { get; set; }
    }
}
