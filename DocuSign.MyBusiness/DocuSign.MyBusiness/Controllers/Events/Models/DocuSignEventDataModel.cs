namespace DocuSign.MyBusiness.Controllers.Events.Model
{
    public class DocuSignEventDataModel
    {
        public string EnvelopeId { get; set; }
        public string AccountId { get; set; }
        public string UserId { get; set; }
        public DocuSignEventEnvelopeSummaryModel EnvelopeSummary { get; set; }
    }
}