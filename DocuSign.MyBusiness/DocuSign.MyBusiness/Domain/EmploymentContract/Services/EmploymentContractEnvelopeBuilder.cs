using DocuSign.eSign.Model;
using System.Collections.Generic;
using System;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Models;
using DocuSign.MyBusiness.Domain.EmploymentContract.Services.Interfaces;

namespace DocuSign.MyBusiness.Domain.EmploymentContract.Services
{
    public class EmploymentContractEnvelopeBuilder : IEmploymentContractEnvelopeBuilder
    {
        public EnvelopeDefinition BuildEmploymentContractEnvelope(
            string eventNotificationUrl,
            EnvelopeAction envelopAction,
            string serverTemplateId,
            SignerInfo signerInfo,
            SignatureInfo signatureInfo)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "REST example - template and digital signature",
                EnableWetSign = "false",
                Status = GetEnvelopStatusByAction(envelopAction),
                EventNotifications = ConfigureEventNotifications(eventNotificationUrl),
                CompositeTemplates = new List<CompositeTemplate>
                {
                    new CompositeTemplate
                    {
                        ServerTemplates = new List<ServerTemplate>
                        {
                            new ServerTemplate
                            {
                                Sequence = "1",
                                TemplateId = serverTemplateId,
                            }
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "2",
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer>
                                    {
                                        new Signer
                                        {
                                            Email = signerInfo.Email,
                                            Name = signerInfo.FullName,
                                            RecipientId = "1",
                                            RoleName = "Candidate",
                                            RecipientSignatureProviders = GetSignatureProviders(signatureInfo)
                                        }

                                    }
                                },
                            },
                        }
                    }
                }
            };

            return envelopeDefinition;
        }

        private static List<EventNotification> ConfigureEventNotifications(string eventNotificationUrl)
        {
            return new List<EventNotification>
                {
                    new EventNotification
                    {
                        Url = eventNotificationUrl,
                        DeliveryMode = "SIM",
                        RequireAcknowledgment = "true",
                        LoggingEnabled = "true",
                        Events = new List<string>
                        {
                            "envelope-sent",
                            "envelope-completed",
                            "envelope-declined",
                            "envelope-voided",
                        },
                        EventData = new ConnectEventData
                        {
                            Version = "restv2.1",
                            Format = "json",
                            IncludeData = new List<string>
                            {
                                "recipients",
                                "tabs"
                            }
                        },
                    }
                };
        }

        private static List<RecipientSignatureProvider> GetSignatureProviders(SignatureInfo signatureInfo)
        {
            switch (signatureInfo.ProviderName)
            {
                case SignatureInfo.DsEuAdvancedProviderName:
                    return new List<RecipientSignatureProvider>
                                            {
                                                new RecipientSignatureProvider
                                                {
                                                    SignatureProviderName = signatureInfo.ProviderName,
                                                    SignatureProviderOptions = new RecipientSignatureProviderOptions
                                                    {
                                                        OneTimePassword = signatureInfo.AccessCode
                                                    }
                                                }
                                            };
                case SignatureInfo.DsElectronicProviderName:
                case SignatureInfo.DsEmailProviderName:
                case SignatureInfo.DefaultProviderName:
                    return new List<RecipientSignatureProvider>();
                default:
                    throw new ApplicationException("The signature provider is not supported");
            }
        }

        private string GetEnvelopStatusByAction(EnvelopeAction envelopAction)
        {
            switch (envelopAction)
            {
                case EnvelopeAction.Send:
                    return "sent";
                case EnvelopeAction.ReviewAndSend:
                    return "created";
                default:
                    throw new ApplicationException("The envelop action is not supported");
            }
        }
    }
}
