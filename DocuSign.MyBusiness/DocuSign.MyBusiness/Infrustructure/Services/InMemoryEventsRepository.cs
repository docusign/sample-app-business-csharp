using System.Collections.Generic;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    public class InMemoryEventsRepository : IEventsRepository
    {
        private IDictionary<string, string> _tokensToEventReceiverDictionary = new Dictionary<string, string>();
        private IDictionary<string, (string, string)> _envelopIdToReceiverAndUseCaseTypes = new Dictionary<string, (string, string)>();

        public void SaveReceiver(string accessToken, string connectionId)
        {
            _tokensToEventReceiverDictionary[accessToken] = connectionId;
        }

        public void SaveEnvelope(string accessToken, string envelopeId, string useCaseType)
        {
            _envelopIdToReceiverAndUseCaseTypes[envelopeId] = (accessToken, useCaseType);
        }

        public (string connectionId, string useCaseType) GetEnvelopDetails(string envelopeId)
        {
            (string accessToken, string useCaseType) = _envelopIdToReceiverAndUseCaseTypes[envelopeId];
            string connectionId = _tokensToEventReceiverDictionary[accessToken];
            return (connectionId, useCaseType);
        }

        public bool IsEnvelopRegistered(string envelopeId)
        {
            return _envelopIdToReceiverAndUseCaseTypes.ContainsKey(envelopeId);
        }
    }
}