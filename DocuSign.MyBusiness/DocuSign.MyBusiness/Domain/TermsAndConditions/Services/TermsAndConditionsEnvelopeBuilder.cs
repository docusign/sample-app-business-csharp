using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Domain.Common.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Models;
using DocuSign.MyBusiness.Domain.TermsAndConditions.Services.Interfaces;
using System.Collections.Generic;

namespace DocuSign.MyBusiness.Domain.TermsAndConditions.Services
{
    public class TermsAndConditionsEnvelopeBuilder : ITermsAndConditionsEnvelopeBuilder
    {
        public EnvelopeDefinition BuildTermsAndConditionsWithContractEnvelope(
            string eventNotificationUrl,
            string contractTemplateId,
            string termsAndConditionsTemplateId,
            SignerInfo signerInfo,
            CarbonCopyInfo carbonCopyInfo)
        {
            EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = $"{signerInfo.FullName}, please review and sign.",
                Status = "sent",
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
                                TemplateId = termsAndConditionsTemplateId,
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
                                            RoleName = "Customer",
                                            AccessCode = signerInfo.AccessCode,
                                            RecipientId = "1",
                                        }

                                    },
                                    CarbonCopies = new List<CarbonCopy>

                                    {
                                        new CarbonCopy
                                        {
                                            Email= carbonCopyInfo.Email,
                                            Name= carbonCopyInfo.FullName,
                                            RecipientId= "2",
                                            RoutingOrder= "2",
                                        }
                                    }
                                },
                            },
                        }
                    },
                    new CompositeTemplate
                    {
                        ServerTemplates = new List<ServerTemplate>
                        {
                            new ServerTemplate
                            {
                                Sequence = "1",
                                TemplateId = contractTemplateId,
                            }
                        },
                        InlineTemplates = new List<InlineTemplate>
                        {
                            new InlineTemplate
                            {
                                Sequence = "2",
                                CustomFields = new CustomFields
                                {
                                    TextCustomFields = new List<TextCustomField>
                                    {
                                        new TextCustomField
                                        {
                                            Name = "Opportunity.ID",
                                            Value = "NE",
                                            Show = "true"
                                        }
                                    }
                                },
                                Recipients = new Recipients
                                {
                                    Signers = new List<Signer>
                                    {
                                        new Signer
                                        {
                                            Email = signerInfo.Email,
                                            Name = signerInfo.FullName,
                                            RoleName = "Customer",
                                            AccessCode = signerInfo.AccessCode,
                                            RecipientId = "1",
                                            Tabs = new Tabs
                                            {
                                                TextTabs = new List<Text>
                                                {
                                                    new Text
                                                    {
                                                        TabLabel = "Customer.Quantity",
                                                        Value= "200"
                                                    },
                                                    new Text
                                                    {
                                                        TabLabel= "Customer.Support.Term",
                                                        Value = "2 years"
                                                    }
                                                },
                                                NumberTabs = new List<Number>
                                                {
                                                    new Number
                                                    {
                                                        TabLabel = "Customer.Term",
                                                        Value = "3"
                                                    }
                                                }
                                            }
                                        }

                                    },
                                    CarbonCopies = new List<CarbonCopy>
                                    {
                                        new CarbonCopy
                                        {
                                            Email= carbonCopyInfo.Email,
                                            Name= carbonCopyInfo.FullName,
                                            RecipientId= "2",
                                            RoutingOrder= "2",
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
    }
}
