using System;

namespace DocuSign.MyBusiness.Controllers.Events.Model
{
    public class EventModel
    {
        public string UseCase { get; set; }
        public string Event { get; set; }
        public DateTime Date { get; set; }
        public SignerModel Signer { get; set; }
    }
}
