using System;

namespace DocuSign.MyBusiness.Controllers.Events.Model
{
    public class DocuSignEventModel
    {
        public string Event { get; set; }
        public DateTime GeneratedDateTime { get; set; }
        public DocuSignEventDataModel Data { get; set; }
    }
}
