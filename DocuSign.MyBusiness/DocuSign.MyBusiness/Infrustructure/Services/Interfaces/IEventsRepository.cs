namespace DocuSign.MyBusiness.Infrustructure.Services.Interfaces
{
    public interface IEventsRepository
    {
        (string connectionId, string useCaseType) GetEnvelopDetails(string envelopeId);
        bool IsEnvelopRegistered(string envelopeId);
        void SaveEnvelope(string accessToken, string envelopeId, string useCaseType);
        void SaveReceiver(string accessToken, string connectionId);
    }
}